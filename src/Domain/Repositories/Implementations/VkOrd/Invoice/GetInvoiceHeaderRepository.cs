using Domain.Repositories.Interfaces.VkOrd.Invoice;
using Domain.Services.Interfaces;
using Domain.VkOrdApi.Invoice;

namespace Domain.Repositories.Implementations.VkOrd.Invoice;

/// <summary>
/// Репозиторий для получения заголовка акта VK ORD API
/// </summary>
public class GetInvoiceHeaderRepository : IGetInvoiceHeaderRepository
{
    private readonly IVkOrdApiClientFactory _vkOrdClientFactory;
    private readonly ILogger<GetInvoiceHeaderRepository> _logger;

    public GetInvoiceHeaderRepository(
        IVkOrdApiClientFactory vkOrdClientFactory,
        ILogger<GetInvoiceHeaderRepository> logger)
    {
        _vkOrdClientFactory = vkOrdClientFactory;
        _logger = logger;
    }

    public async Task<VkOrdApiInvoiceHeaderResponse> GetHeader(
        string externalId,
        CancellationToken cancellationToken)
    {
        var vkOrdClient = await _vkOrdClientFactory.CreateClient();

        var response = await vkOrdClient.GetInvoiceHeaderV3(externalId, cancellationToken);

        _logger.LogInformation($"Invoice header {externalId} retrieved from VK ORD API");

        return response;
    }
}
