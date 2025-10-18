using Domain.Data;
using Domain.VkOrdApi;
using Domain.VkOrdApi.Statistics;
using WebApp.Models.Responses;
using WebApp.Repositories.Interfaces.VkOrd.Statistics;
using WebApp.Services.Interfaces;

namespace WebApp.Repositories.Implementations.VkOrd.Statistics;

/// <summary>
/// Репозиторий для получения списка статистик
/// </summary>
public class GetStatisticsListRepository : IGetStatisticsListRepository
{
    private readonly IVkOrdApiClient _vkOrdClient;
    private readonly IVkOrdApiClientFactory _vkOrdApiClientFactory;
    private readonly AppDbContext _context;
    private readonly ILogger<GetStatisticsListRepository> _logger;

    public GetStatisticsListRepository(
        IVkOrdApiClientFactory vkOrdApiClientFactory,
        AppDbContext context,
        ILogger<GetStatisticsListRepository> logger)
    {
        _vkOrdClient = vkOrdApiClientFactory.CreateClient().GetAwaiter().GetResult();
        _vkOrdApiClientFactory = vkOrdApiClientFactory;
        _context = context;
        _logger = logger;
    }

    public async Task<GetStatisticsDto> GetListAsync(
        string? creativeExternalId = null,
        string? padExternalId = null,
        int offset = 0,
        int limit = 100,
        CancellationToken cancellationToken = default)
    {
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
        var response = await _vkOrdClient.GetStatisticsListV2(request, cancellationToken);

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
