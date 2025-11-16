using Domain.Data;
using Domain.Models.Responses;
using Domain.Repositories.Interfaces.VkOrd.Statistics;
using Domain.Services.Interfaces;
using Domain.VkOrdApi.Statistics;

namespace Domain.Repositories.Implementations.VkOrd.Statistics;

/// <summary>
/// Репозиторий для получения списка статистик
/// </summary>
public class GetStatisticsListRepository : IGetStatisticsListRepository
{
    private readonly IVkOrdApiClientFactory _vkOrdApiClientFactory;
    private readonly ILogger<GetStatisticsListRepository> _logger;

    public GetStatisticsListRepository(
        IVkOrdApiClientFactory vkOrdApiClientFactory,
        ILogger<GetStatisticsListRepository> logger)
    {
        _vkOrdApiClientFactory = vkOrdApiClientFactory;
        _logger = logger;
    }

    public async Task<GetStatisticsDto> GetListAsync(
        CancellationToken cancellationToken,
        string? creativeExternalId = null,
        string? padExternalId = null,
        int offset = 0,
        int limit = 100)
    {
        var vkOrdClient = await _vkOrdApiClientFactory.CreateClient();
        var vkOrdCredential = await _vkOrdApiClientFactory.GetVkOrdCredentialAsync();
        var logicalAccountId = vkOrdCredential.LogicalAccountId;

        // Создаем запрос для VK ORD API
        var request = new VkOrdApiStatisticsListRequest
        {
            CreativeExternalId = creativeExternalId,
            PadExternalId = padExternalId,
            Offset = offset,
            Limit = limit
        };

        // Получаем список из VK ORD API (GET /v2/statistics/list)
        var response = await vkOrdClient.GetStatisticsListV2(request, cancellationToken);

        _logger.LogInformation(
            "Successfully fetched {Count} statistics from VK ORD API for LogicalAccountId: {LogicalAccountId}",
            response.Items.Count, logicalAccountId);

        return new GetStatisticsDto
        {
            Data = response.Items,
            TotalItemsCount = response.TotalItemsCount,
            Limit = response.Limit
        };
    }
}
