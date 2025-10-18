using Domain;
using Domain.VkOrdApi.Invoice;
using WebApp.Repositories.Interfaces.VkOrd.Invoice;
using WebApp.Services.Interfaces;

namespace WebApp.Repositories.Implementations.VkOrd.Invoice;

/// <summary>
/// Репозиторий для получения списка актов VK ORD API с пагинацией
/// </summary>
public class GetPageInvoiceRepository : IGetPageInvoiceRepository
{
    private readonly IVkOrdApiClientFactory _vkOrdClientFactory;
    private readonly ILogger<GetPageInvoiceRepository> _logger;

    public GetPageInvoiceRepository(
        IVkOrdApiClientFactory vkOrdClientFactory,
        ILogger<GetPageInvoiceRepository> logger)
    {
        _vkOrdClientFactory = vkOrdClientFactory;
        _logger = logger;
    }

    public async Task<VkOrdApiInvoiceListResponse> Get(PageRequest pageRequest, CancellationToken cancellationToken)
    {
        var vkOrdClient = await _vkOrdClientFactory.CreateClient();

        try
        {
            var response = await vkOrdClient.GetInvoicesV1(pageRequest, cancellationToken);

            _logger.LogInformation(
                $"VK ORD API response - ExternalIds count: {response?.ExternalIds?.Count ?? 0}, " +
                $"TotalItemsCount: {response?.TotalItemsCount}, Limit: {response?.Limit}");

            return response ?? new VkOrdApiInvoiceListResponse();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting invoices page from VK ORD API");
            throw;
        }
    }
}
