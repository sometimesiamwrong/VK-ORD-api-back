using Domain.Repositories.Interfaces.VkOrd.Invoice;
using Domain.Services.Interfaces;
using Domain.VkOrdApi.Invoice;

namespace Domain.Repositories.Implementations.VkOrd.Invoice;

/// <summary>
/// Репозиторий для отправки акта в ЕРИР
/// </summary>
public class SendInvoiceToErirRepository : ISendInvoiceToErirRepository
{
    private readonly IVkOrdApiClientFactory _vkOrdClientFactory;
    private readonly ILogger<SendInvoiceToErirRepository> _logger;

    public SendInvoiceToErirRepository(
        IVkOrdApiClientFactory vkOrdClientFactory,
        ILogger<SendInvoiceToErirRepository> logger)
    {
        _vkOrdClientFactory = vkOrdClientFactory;
        _logger = logger;
    }

    public async Task Send(string externalId, CancellationToken cancellationToken)
    {
        var vkOrdClient = await _vkOrdClientFactory.CreateClient();

        var request = new VkOrdApiSendInvoiceToErirRequest();
        await vkOrdClient.SendInvoiceToErirV2(externalId, request, cancellationToken);

        _logger.LogInformation($"Invoice {externalId} sent to ERIR in VK ORD API");
    }
}
