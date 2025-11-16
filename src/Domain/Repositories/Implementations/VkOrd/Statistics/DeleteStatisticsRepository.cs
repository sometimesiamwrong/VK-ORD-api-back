using Domain.Data;
using Domain.Entities.Enums;
using Domain.Entities.VkOrd;
using Domain.Repositories.Interfaces.VkOrd.Statistics;
using Domain.Services.Interfaces;
using Domain.VkOrdApi.Statistics;
using Microsoft.EntityFrameworkCore;

namespace Domain.Repositories.Implementations.VkOrd.Statistics;

/// <summary>
/// Репозиторий для удаления статистики
/// </summary>
public class DeleteStatisticsRepository : IDeleteStatisticsRepository
{
    private readonly IVkOrdApiClientFactory _vkOrdApiClientFactory;
    private readonly Func<AppDbContext> _contextFactory;
    private readonly ICacheService _cacheService;
    private readonly ILogger<DeleteStatisticsRepository> _logger;

    public DeleteStatisticsRepository(
        IVkOrdApiClientFactory vkOrdApiClientFactory,
        Func<AppDbContext> contextFactory,
        ICacheService cacheService,
        ILogger<DeleteStatisticsRepository> logger)
    {
        _vkOrdApiClientFactory = vkOrdApiClientFactory;
        _contextFactory = contextFactory;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task DeleteAsync(
        string creativeExternalId,
        string padExternalId,
        string dateStartActual,
        CancellationToken cancellationToken)
    {
        await using var context = _contextFactory();
        var vkOrdCredential = await _vkOrdApiClientFactory.GetVkOrdCredentialAsync();
        var logicalAccountId = vkOrdCredential.LogicalAccountId;
        var vkOrdClient = await _vkOrdApiClientFactory.CreateClient();

        // Создаем запрос для VK ORD API
        var request = new VkOrdApiDeleteStatisticsRequest
        {
            Items = new List<VkOrdApiDeleteStatisticsItem>
            {
                new VkOrdApiDeleteStatisticsItem
                {
                    CreativeExternalId = creativeExternalId,
                    PadExternalId = padExternalId,
                    DateStartActual = dateStartActual
                }
            }
        };

        // Удаляем через VK ORD API (POST /v1/statistics/delete)
        await vkOrdClient.DeleteStatisticsV1(request, cancellationToken);

        _logger.LogInformation(
            "Successfully deleted statistic from VK ORD API. CreativeExternalId: {CreativeExternalId}, " +
            "PadExternalId: {PadExternalId}, DateStartActual: {DateStartActual}, LogicalAccountId: {LogicalAccountId}",
            creativeExternalId, padExternalId, dateStartActual, logicalAccountId);

        // Удаляем из локальной БД
        // Преобразуем строковую дату в DateTime для поиска
        DateTime? dateStartActualParsed = DateTime.TryParse(dateStartActual, out var parsedDate) 
            ? parsedDate 
            : null;

        var statistic = await context.VkOrdStatistics
            .FirstOrDefaultAsync(
                s => s.LogicalAccountId == logicalAccountId &&
                     s.CreativeExternalId == creativeExternalId &&
                     s.PadExternalId == padExternalId &&
                     s.DateStartActual == dateStartActualParsed,
                cancellationToken);

        if (statistic != null)
        {
            context.VkOrdStatistics.Remove(statistic);
            await context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Successfully deleted statistic from database. Id: {Id}", statistic.Id);
        }

        // Инвалидируем кеш
        var cacheKey = GetCacheKey(logicalAccountId, creativeExternalId, padExternalId, dateStartActual);
        await _cacheService.Remove<VkOrdStatistic>(cacheKey, cancellationToken);
    }

    private static string GetCacheKey(long logicalAccountId, string creativeExternalId, string padExternalId, string dateStartActual)
    {
        return $"vkord:{logicalAccountId}:{creativeExternalId}:{padExternalId}:{dateStartActual}:{EntityType.Statistic}";
    }
}
