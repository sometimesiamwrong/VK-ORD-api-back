using Domain.VkOrdApi.Invoice;
using WebApp.Repositories.Interfaces.VkOrd.Invoice;
using WebApp.Services.Interfaces;

namespace WebApp.Repositories.Implementations.VkOrd.Invoice;

/// <summary>
/// Репозиторий для удаления договоров из акта VK ORD API
/// </summary>
public class DeleteContractsFromInvoiceRepository : IDeleteContractsFromInvoiceRepository
{
    private readonly IVkOrdApiClientFactory _vkOrdClientFactory;
    private readonly ILogger<DeleteContractsFromInvoiceRepository> _logger;

    public DeleteContractsFromInvoiceRepository(
        IVkOrdApiClientFactory vkOrdClientFactory,
        ILogger<DeleteContractsFromInvoiceRepository> logger)
    {
        _vkOrdClientFactory = vkOrdClientFactory;
        _logger = logger;
    }

    public async Task DeleteContracts(
        string externalId,
        VkOrdApiDeleteContractsFromInvoiceRequest request,
        CancellationToken cancellationToken)
    {
        var vkOrdClient = await _vkOrdClientFactory.CreateClient();

        await vkOrdClient.DeleteContractsFromInvoiceV2(externalId, request, cancellationToken);

        _logger.LogInformation($"Contracts deleted from invoice {externalId} in VK ORD API");
    }
}
