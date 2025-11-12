using System.Globalization;
using Domain.BrokenRules;
using Domain.Data;
using Domain.Entities;
using Domain.Entities.Enums;
using Domain.Entities.Enums.VkOrd;
using Domain.Entities.VkOrd;
using Domain.Extensions;
using Domain.Models.Requests;
using Domain.Repositories.Interfaces.VkOrd.Invoice;
using Domain.Services.Interfaces;
using Domain.VkOrdApi.Invoice;
using Microsoft.EntityFrameworkCore;

namespace Domain.Repositories.Implementations.VkOrd.Invoice;

/// <summary>
/// Репозиторий для создания/обновления актов VK ORD API
/// </summary>
public class CreateOrUpdateInvoiceRepository : ICreateOrUpdateInvoiceRepository
{
    private readonly IVkOrdApiClientFactory _vkOrdClientFactory;
    private readonly ICacheService _cacheService;
    private readonly AppDbContext _context;
    private readonly ILogger<CreateOrUpdateInvoiceRepository> _logger;

    public CreateOrUpdateInvoiceRepository(
        IVkOrdApiClientFactory vkOrdClientFactory,
        ICacheService cacheService,
        AppDbContext context,
        ILogger<CreateOrUpdateInvoiceRepository> logger)
    {
        _vkOrdClientFactory = vkOrdClientFactory;
        _cacheService = cacheService;
        _context = context;
        _logger = logger;
    }

    public async Task<VkOrdInvoice> CreateOrUpdateInvoice(
        string externalId,
        CreateOrUpdateInvoiceRequest request,
        bool isDraft,
        CancellationToken cancellationToken)
    {
        var vkOrdCredential = await _vkOrdClientFactory.GetVkOrdCredentialAsync();

        // Проверяем, существует ли акт в БД
        var existingInvoice = await _context.VkOrdInvoices
            .FirstOrDefaultAsync(
                AppDbContext.DefaultGetVkOrd<VkOrdInvoice>(externalId, vkOrdCredential),
                cancellationToken);

        var isUpdate = existingInvoice != null;

        if (isUpdate)
        {
            _logger.LogInformation($"Updating existing invoice {externalId}");
        }
        else
        {
            _logger.LogInformation($"Creating new invoice {externalId}");
        }

        // Маппинг request в API модель
        var apiRequest = MapToApiRequest(request);

        var vatRate = decimal.Parse(apiRequest.Amount.Services.VatRate, CultureInfo.InvariantCulture);
        var includingVat = decimal.Parse(request.Amount.Services.IncludingVat, CultureInfo.InvariantCulture);
        
        var calculatedExcludingVat = Math.Round(includingVat / (1 + vatRate / 100), 2);
        var calculatedVat = Math.Round(includingVat - calculatedExcludingVat, 2);

        if (includingVat != calculatedExcludingVat + calculatedVat)
        {
            calculatedVat += 0.01m;
            calculatedExcludingVat = includingVat - calculatedVat;
        }

        apiRequest.Amount.Services.ExcludingVat = calculatedExcludingVat.ToString(CultureInfo.InvariantCulture);
        apiRequest.Amount.Services.Vat = calculatedVat.ToString(CultureInfo.InvariantCulture);
        apiRequest.Amount.Services.IncludingVat = includingVat.ToString(CultureInfo.InvariantCulture);
        apiRequest.Amount.Services.VatRate = vatRate.ToString(CultureInfo.InvariantCulture);

        if (apiRequest.Items != null)
        {
            foreach (var item in apiRequest.Items)
            {
                var itemVatRate = decimal.Parse(item.Amount.VatRate, CultureInfo.InvariantCulture);
                var itemIncludingVat = decimal.Parse(item.Amount.IncludingVat, CultureInfo.InvariantCulture);

                var itemCalculatedExcludingVat = Math.Round(itemIncludingVat / (1 + itemVatRate / 100), 2);
                var itemCalculatedVat = Math.Round(itemIncludingVat - itemCalculatedExcludingVat, 2);

                if (itemIncludingVat != itemCalculatedExcludingVat + itemCalculatedVat)
                {
                    itemCalculatedVat += 0.01m;
                    itemCalculatedExcludingVat = itemIncludingVat - itemCalculatedVat;
                }

                item.Amount.ExcludingVat = itemCalculatedExcludingVat.ToString(CultureInfo.InvariantCulture);
                item.Amount.Vat = itemCalculatedVat.ToString(CultureInfo.InvariantCulture);
                item.Amount.IncludingVat = itemIncludingVat.ToString(CultureInfo.InvariantCulture);
                item.Amount.VatRate = itemVatRate.ToString(CultureInfo.InvariantCulture);
            }
        }

        _logger.LogInformation($"Adjusted invoice amounts - VAT: {calculatedVat}, Including VAT: {includingVat}, Excluding VAT: {calculatedExcludingVat}");


        // Вызов VK Ord API
        var vkOrdClient = await _vkOrdClientFactory.CreateClient();
        
        await vkOrdClient.CreateFullInvoiceV3(
            externalId,
            draft: request.Status == VkOrdApiInvoiceStatus.Draft,
            apiRequest,
            cancellationToken);

        _logger.LogInformation($"Invoice {externalId} successfully sent to VK ORD API");

        // Получаем обновленные данные из API
        var invoiceResponse = await vkOrdClient.GetFullInvoiceV3(externalId, cancellationToken);

        if (invoiceResponse == null)
        {
            throw BrokenRuleCodes.DataIsEmpty.AsExn();
        }

        // Создаем или обновляем entity
        var invoice = existingInvoice ?? new VkOrdInvoice
        {
            LogicalAccountId = vkOrdCredential.LogicalAccountId,
            ExternalId = externalId,
            ContractExternalId = invoiceResponse.ContractExternalId,
        };

        invoice.UpdateData(invoiceResponse);

        if (!isUpdate)
        {
            invoice.CreatedAt = DateTimeOffset.UtcNow;
        }
        invoice.UpdatedAt = DateTimeOffset.UtcNow;
        
        // Сохраняем в БД
        await SaveToDatabase(invoice, isUpdate, cancellationToken);

        // Инвалидируем кэш
        var cacheKey = GetCacheKey(externalId, vkOrdCredential);
        await _cacheService.Remove<VkOrdInvoice>(cacheKey, cancellationToken);

        _logger.LogInformation($"Invoice {externalId} saved to database");

        return invoice;
    }

    private VkOrdApiFullInvoiceRequest MapToApiRequest(CreateOrUpdateInvoiceRequest request)
    {
        return new VkOrdApiFullInvoiceRequest
        {
            ContractExternalId = request.ContractExternalId,
            OrderContractExternalId = request.OrderContractExternalId,
            Date = request.Date,
            Serial = request.Serial,
            DateStart = request.DateStart,
            DateEnd = request.DateEnd,
            Amount = request.Amount,
            ApiClientRole = request.ClientRole,
            ContractorRole = request.ContractorRole,
            Items = request.Items
        };
    }

    private async Task SaveToDatabase(VkOrdInvoice invoice, bool isUpdate, CancellationToken cancellationToken)
    {
        if (isUpdate)
        {
            _context.VkOrdInvoices.Update(invoice);
        }
        else
        {
            await _context.VkOrdInvoices.AddAsync(invoice, cancellationToken);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    private string GetCacheKey(string externalId, ApiCredential apiCredential)
    {
        return $"vkord:{apiCredential.LogicalAccountId}:{externalId}:{EntityType.Invoice.GetDescription()}";
    }
}
