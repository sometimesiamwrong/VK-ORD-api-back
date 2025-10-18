using Domain.Data;
using Domain.Entities.Enums;
using Domain.Entities.VkOrd;
using Domain.VkOrdApi;
using Domain.VkOrdApi.Statistics;
using Microsoft.EntityFrameworkCore;
using WebApp.Repositories.Interfaces.VkOrd.Statistics;
using WebApp.Services.Interfaces;

namespace WebApp.Repositories.Implementations.VkOrd.Statistics;

/// <summary>
/// Репозиторий для создания/обновления статистики
/// </summary>
public class CreateOrUpdateStatisticsRepository : ICreateOrUpdateStatisticsRepository
{
    private readonly IVkOrdApiClient _vkOrdClient;
    private readonly IVkOrdApiClientFactory _vkOrdApiClientFactory;
    private readonly AppDbContext _context;
    private readonly ICacheService _cacheService;
    private readonly ILogger<CreateOrUpdateStatisticsRepository> _logger;

    public CreateOrUpdateStatisticsRepository(
        IVkOrdApiClientFactory vkOrdApiClientFactory,
        AppDbContext context,
        ICacheService cacheService,
        ILogger<CreateOrUpdateStatisticsRepository> logger)
    {
        _vkOrdClient = vkOrdApiClientFactory.CreateClient().GetAwaiter().GetResult();
        _vkOrdApiClientFactory = vkOrdApiClientFactory;
        _context = context;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task CreateOrUpdateAsync(
        List<VkOrdApiStatisticsItem> items,
        CancellationToken cancellationToken = default)
    {
        var vkOrdCredential = await _vkOrdApiClientFactory.GetVkOrdCredentialAsync();
        var logicalAccountId = vkOrdCredential.LogicalAccountId;

        // Отправляем запрос в VK ORD API (POST /v2/statistics)
        await _vkOrdClient.CreateStatisticsV2(items, cancellationToken);

        _logger.LogInformation(
            "Successfully created/updated {Count} statistics in VK ORD API for LogicalAccountId: {LogicalAccountId}",
            items.Count, logicalAccountId);

        // Сохраняем или обновляем в локальной БД
        foreach (var item in items)
        {
            // Преобразуем строковую дату в DateTime
            DateTime? dateStartActual = DateTime.TryParse(item.DateStartActual, out var parsedDate) 
                ? parsedDate 
                : null;

            var existing = await _context.VkOrdStatistics
                .FirstOrDefaultAsync(
                    s => s.LogicalAccountId == logicalAccountId &&
                         s.CreativeExternalId == item.CreativeExternalId &&
                         s.PadExternalId == item.PadExternalId &&
                         s.DateStartActual == dateStartActual,
                    cancellationToken);

            if (existing != null)
            {
                // Обновляем существующую
                UpdateStatisticsFromItem(existing, item);
                existing.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                // Создаем новую
                var newStatistic = new VkOrdStatistic
                {
                    LogicalAccountId = logicalAccountId,
                    ExternalId = $"{item.CreativeExternalId}_{item.PadExternalId}_{item.DateStartActual}", // Составной ключ
                    CreativeExternalId = item.CreativeExternalId,
                    PadExternalId = item.PadExternalId,
                    DateStartActual = dateStartActual,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                UpdateStatisticsFromItem(newStatistic, item);

                await _context.VkOrdStatistics.AddAsync(newStatistic, cancellationToken);
            }

            // Инвалидируем кеш
            var cacheKey = GetCacheKey(logicalAccountId, item.CreativeExternalId, item.PadExternalId, item.DateStartActual);
            await _cacheService.Remove<VkOrdStatistic>(cacheKey, cancellationToken);
        }

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Successfully saved {Count} statistics to database for LogicalAccountId: {LogicalAccountId}",
            items.Count, logicalAccountId);
    }

    private static string GetCacheKey(long logicalAccountId, string creativeExternalId, string padExternalId, string dateStartActual)
    {
        return $"vkord:{logicalAccountId}:{creativeExternalId}:{padExternalId}:{dateStartActual}:{EntityType.Statistic}";
    }

    private static void UpdateStatisticsFromItem(VkOrdStatistic entity, VkOrdApiStatisticsItem item)
    {
        entity.ShowsCount = item.ShowsCount;
        entity.InvoiceShowsCount = item.InvoiceShowsCount;
        entity.Amount = System.Text.Json.JsonSerializer.Serialize(item.Amount);
        entity.AmountPerEvent = decimal.TryParse(item.AmountPerEvent, out var amount) ? amount : 0;
        entity.PayType = item.ApiPayType.ToString();
        
        entity.DateStartPlanned = DateTime.TryParse(item.DateStartPlanned, out var startPlanned) ? startPlanned : null;
        entity.DateEndPlanned = DateTime.TryParse(item.DateEndPlanned, out var endPlanned) ? endPlanned : null;
        entity.DateStartActual = DateTime.TryParse(item.DateStartActual, out var startActual) ? startActual : null;
        entity.DateEndActual = DateTime.TryParse(item.DateEndActual, out var endActual) ? endActual : null;
        
        entity.SyncStatus = VkOrdSyncStatus.Synced;
    }
}
