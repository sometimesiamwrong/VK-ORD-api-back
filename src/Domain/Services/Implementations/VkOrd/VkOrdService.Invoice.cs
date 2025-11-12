using Domain.Entities.VkOrd;
using Domain.Models.Requests;
using Domain.Models.Responses;
using Domain.Services.Interfaces;
using Domain.VkOrdApi.Invoice;

namespace Domain.Services.Implementations.VkOrd;

/// <summary>
/// Сервис для работы с актами VK ОРД
/// </summary>
public partial class VkOrdService : IVkOrdService
{
    public async Task CreateOrUpdateInvoice(
        CreateOrUpdateInvoiceRequest request,
        bool isDraft,
        CancellationToken cancellationToken)
    {
        await _createOrUpdateInvoiceRepository.CreateOrUpdateInvoice(
            request.ExternalId,
            request,
            isDraft,
            cancellationToken);

        await GetInvoice(request.ExternalId, cancellationToken);
    }

    public async Task<VkOrdInvoice> GetInvoice(string externalId, CancellationToken cancellationToken)
    {
        return await _getInvoiceRepository.Get(externalId, cancellationToken);
    }

    public async Task<GetInvoicesDto> GetPageInvoice(PageRequest pageRequest, CancellationToken cancellationToken, List<string>? counterpartyExternalIds = null)
    {
        var pageInvoiceListResponse = await _getPageInvoiceRepository.Get(pageRequest, cancellationToken, counterpartyExternalIds);

        if (pageInvoiceListResponse?.ExternalIds == null || pageInvoiceListResponse.ExternalIds.Count == 0)
        {
            return new GetInvoicesDto
            {
                Data = new List<VkOrdInvoice>(),
                TotalItemsCount = 0,
                Limit = 0
            };
        }

        _logger.LogInformation(
            $"VK ORD API response - ExternalIds count: {pageInvoiceListResponse?.ExternalIds?.Count ?? 0}, " +
            $"TotalItemsCount: {pageInvoiceListResponse?.TotalItemsCount}, Limit: {pageInvoiceListResponse?.Limit}");

        var externalIds = pageInvoiceListResponse!.ExternalIds;
        var totalItemsCount = pageInvoiceListResponse.TotalItemsCount;
        var responseLimit = pageInvoiceListResponse.Limit;

        _logger.LogInformation(
            $"Found {externalIds.Count} invoices (total: {totalItemsCount}, responseLimit: {responseLimit}), " +
            $"fetching full data for each");

        // Получаем полные данные для каждого акта последовательно
        var invoices = new List<VkOrdInvoice>();

        foreach (var externalId in externalIds)
        {
            try
            {
                var invoiceResponse = await _getInvoiceRepository.Get(externalId, cancellationToken);
                if (invoiceResponse != null)
                {
                    invoices.Add(invoiceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting invoice {externalId}");
            }
        }

        _logger.LogInformation($"Successfully fetched {invoices.Count} out of {externalIds.Count} invoices");

        return new GetInvoicesDto
        {
            Data = invoices,
            TotalItemsCount = totalItemsCount,
            Limit = responseLimit
        };
    }

    public async Task DeleteInvoice(string externalId, CancellationToken cancellationToken)
    {
        await _deleteInvoiceRepository.Delete(externalId, cancellationToken);
        _logger.LogInformation($"Invoice {externalId} deleted successfully");
    }

    public async Task AddContractsToInvoice(
        string externalId,
        VkOrdApiAddContractsToInvoiceRequest request,
        CancellationToken cancellationToken)
    {
        await _addContractsToInvoiceRepository.AddContracts(externalId, request, cancellationToken);
        _logger.LogInformation($"Contracts added to invoice {externalId} successfully");
    }

    public async Task DeleteContractsFromInvoice(
        string externalId,
        VkOrdApiDeleteContractsFromInvoiceRequest request,
        CancellationToken cancellationToken)
    {
        await _deleteContractsFromInvoiceRepository.DeleteContracts(externalId, request, cancellationToken);
        _logger.LogInformation($"Contracts deleted from invoice {externalId} successfully");
    }

    public async Task SendInvoiceToErir(string externalId, CancellationToken cancellationToken)
    {
        await _sendInvoiceToErirRepository.Send(externalId, cancellationToken);
        _logger.LogInformation($"Invoice {externalId} sent to ERIR successfully");
    }

    public async Task CreateOrUpdateInvoiceHeader(
        string externalId,
        VkOrdApiInvoiceHeaderRequest request,
        CancellationToken cancellationToken)
    {
        await _createOrUpdateInvoiceHeaderRepository.CreateOrUpdateHeader(externalId, request, cancellationToken);
        _logger.LogInformation($"Invoice header {externalId} created/updated successfully");
    }

    public async Task<VkOrdApiInvoiceHeaderResponse> GetInvoiceHeader(
        string externalId,
        CancellationToken cancellationToken)
    {
        return await _getInvoiceHeaderRepository.GetHeader(externalId, cancellationToken);
    }
}
