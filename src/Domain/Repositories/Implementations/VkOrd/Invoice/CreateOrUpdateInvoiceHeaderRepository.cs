using Domain.Repositories.Interfaces.VkOrd.Invoice;
using Domain.Services.Interfaces;
using Domain.VkOrdApi.Invoice;

namespace Domain.Repositories.Implementations.VkOrd.Invoice;

/// <summary>
/// Репозиторий для создания/обновления заголовка акта VK ORD API
/// </summary>
public class CreateOrUpdateInvoiceHeaderRepository : ICreateOrUpdateInvoiceHeaderRepository
{
    private readonly IVkOrdApiClientFactory _vkOrdClientFactory;
    private readonly ILogger<CreateOrUpdateInvoiceHeaderRepository> _logger;

    public CreateOrUpdateInvoiceHeaderRepository(
        IVkOrdApiClientFactory vkOrdClientFactory,
        ILogger<CreateOrUpdateInvoiceHeaderRepository> logger)
    {
        _vkOrdClientFactory = vkOrdClientFactory;
        _logger = logger;
    }

    public async Task CreateOrUpdateHeader(
        string externalId,
        VkOrdApiInvoiceHeaderRequest request,
        CancellationToken cancellationToken)
    {
        var vkOrdClient = await _vkOrdClientFactory.CreateClient();

        await vkOrdClient.CreateInvoiceHeaderV3(externalId, request, cancellationToken);

        _logger.LogInformation($"Invoice header {externalId} created/updated in VK ORD API");
    }
}
