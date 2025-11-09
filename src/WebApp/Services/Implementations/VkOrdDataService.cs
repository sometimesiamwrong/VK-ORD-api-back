using Domain.BrokenRules;
using Domain.Data;
using Domain.Entities;
using Domain.Entities.VkOrd;
using Domain.Extensions;
using Microsoft.EntityFrameworkCore;
using WebApp.Services.Interfaces;

namespace WebApp.Services.Implementations;

/// <summary>
/// Реализация сервиса для работы с данными VK ORD API с кэшированием
/// </summary>
/// <typeparam name="TEntity">Тип сущности VkOrd</typeparam>
/// <typeparam name="TApiResponse">Тип ответа API</typeparam>
public class VkOrdDataService<TEntity, TApiResponse> : IVkOrdDataService<TEntity, TApiResponse>
    where TEntity : VkOrdBase
    where TApiResponse : class
{
    private readonly IVkOrdApiClientFactory _vkOrdClientFactory;
    private readonly ICacheService _cacheService;
    private readonly AppDbContext _context;
    private readonly ILogger<VkOrdDataService<TEntity, TApiResponse>> _logger;

    public VkOrdDataService(
        IVkOrdApiClientFactory vkOrdClientFactory,
        ICacheService cacheService,
        AppDbContext context,
        ILogger<VkOrdDataService<TEntity, TApiResponse>> logger)
    {
        _vkOrdClientFactory = vkOrdClientFactory;
        _cacheService = cacheService;
        _context = context;
        _logger = logger;
    }

    public async Task<TEntity> GetAsync(
        string identifier,
        Func<string, CancellationToken, Task<TApiResponse?>> getApiOperation,
        Func<TApiResponse, TEntity?, ApiCredential, TEntity> mapOperation,
        Func<DbSet<TEntity>, IQueryable<TEntity>> getFromDatabaseQuery,
        CancellationToken cancellationToken)
    {
        var credential = await _vkOrdClientFactory.GetVkOrdCredentialAsync();
        var cacheKey = GetCacheKey(identifier, credential);

        // Получаем данные из кэша
        var data = await _cacheService.Get<TEntity>(cacheKey, cancellationToken);

        // Если данные не найдены в кэше или устарели, то получаем данные из базы данных
        if (data == null || data.IsExpired())
        {
            // Получаем данные из базы данных
            var query = getFromDatabaseQuery(_context.Set<TEntity>());
            data = await query.FirstOrDefaultAsync(cancellationToken);

            if (data == null || data.IsExpired())
            {
                // Получаем данные из API
                var apiData = await getApiOperation(identifier, cancellationToken);
                if (apiData == null)
                {
                    throw BrokenRuleCodes.DataIsEmpty.AsExn();
                }

                // Мапим данные
                data = mapOperation(apiData, data, credential);

                // Сохраняем данные в базу данных
                await SaveToDatabase(data, cancellationToken);
            }

            // Сохраняем данные в кэш
            await _cacheService.Save(cacheKey, data, cancellationToken);
        }

        return data;
    }

    public async Task<TEntity?> FirstOrDefaultAsync(IQueryable<TEntity> query, CancellationToken cancellationToken)
    {
        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<TEntity>> ToListAsync(IQueryable<TEntity> query, CancellationToken cancellationToken)
    {
        return await query.ToListAsync(cancellationToken);
    }

    private async Task SaveToDatabase(TEntity entity, CancellationToken cancellationToken)
    {
        if (entity.IsNew())
        {
            _context.Set<TEntity>().Add(entity);
        }
        else
        {
            _context.Set<TEntity>().Update(entity);
        }
        await _context.SaveChangesAsync(cancellationToken);
    }

    private string GetCacheKey(string identifier, ApiCredential credential)
    {
        var entityType = typeof(TEntity).Name.Replace("VkOrd", "").ToLower();
        return $"vkord:{entityType}:{credential.LogicalAccountId}:{identifier}";
    }
}
