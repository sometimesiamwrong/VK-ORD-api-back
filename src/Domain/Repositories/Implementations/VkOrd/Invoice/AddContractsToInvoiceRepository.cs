using Domain.Repositories.Interfaces.VkOrd.Invoice;
using Domain.Services.Interfaces;
using Domain.VkOrdApi.Invoice;

namespace Domain.Repositories.Implementations.VkOrd.Invoice;

/// <summary>
/// Репозиторий для добавления договоров в акт VK ORD API
/// </summary>
public class AddContractsToInvoiceRepository : IAddContractsToInvoiceRepository
{
    private readonly IVkOrdApiClientFactory _vkOrdClientFactory;
    private readonly ILogger<AddContractsToInvoiceRepository> _logger;

    public AddContractsToInvoiceRepository(
        IVkOrdApiClientFactory vkOrdClientFactory,
        ILogger<AddContractsToInvoiceRepository> logger)
    {
        _vkOrdClientFactory = vkOrdClientFactory;
        _logger = logger;
    }

    public async Task AddContracts(
        string externalId,
        VkOrdApiAddContractsToInvoiceRequest request,
        CancellationToken cancellationToken)
    {
        var vkOrdClient = await _vkOrdClientFactory.CreateClient();

        await vkOrdClient.AddContractsToInvoiceV3(externalId, request, cancellationToken);

        _logger.LogInformation($"Contracts added to invoice {externalId} in VK ORD API");
    }
}
