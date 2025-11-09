using Domain.VkOrdApi.Statistics;
using WebApp.Models.Requests;
using WebApp.Models.Responses;

namespace WebApp.Services.Implementations.VkOrd;

/// <summary>
/// Часть VkOrdService для работы со статистикой
/// </summary>
public partial class VkOrdService
{
    /// <summary>
    /// Создать или обновить статистику
    /// </summary>
    public async Task CreateOrUpdateStatistics(
        List<VkOrdApiStatisticsItem> items,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating/Updating {Count} statistics", items.Count);

        await _createOrUpdateStatisticsRepository.CreateOrUpdateAsync(
            items,
            cancellationToken);

        _logger.LogInformation("Successfully created/updated {Count} statistics", items.Count);
    }

    /// <summary>
    /// Получить список статистик с фильтрацией и пагинацией
    /// </summary>
    public async Task<GetStatisticsDto> GetStatisticsList(
        string? creativeExternalId,
        string? padExternalId,
        int offset,
        int limit,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Getting statistics list with CreativeExternalId: {CreativeExternalId}, " +
            "PadExternalId: {PadExternalId}, Offset: {Offset}, Limit: {Limit}",
            creativeExternalId, padExternalId, offset, limit);

        var result = await _getStatisticsListRepository.GetListAsync(
            cancellationToken,
            creativeExternalId,
            padExternalId,
            offset,
            limit);

        _logger.LogInformation(
            "Successfully retrieved {Count} statistics (Total: {Total})",
            result.Data.Count, result.TotalItemsCount);

        return result;
    }

    /// <summary>
    /// Удалить статистику
    /// </summary>
    public async Task DeleteStatistics(
        List<DeleteStatisticsItemRequest> items,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting {Count} statistics", items.Count);

        foreach (var item in items)
        {
            await _deleteStatisticsRepository.DeleteAsync(
                item.CreativeExternalId,
                item.PadExternalId,
                item.DateStartActual,
                cancellationToken);
        }

        _logger.LogInformation("Successfully deleted {Count} statistics", items.Count);
    }
}
