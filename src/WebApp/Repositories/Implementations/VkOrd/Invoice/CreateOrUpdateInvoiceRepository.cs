using Domain.BrokenRules;
using Domain.Data;
using Domain.Entities;
using Domain.Entities.Enums;
using Domain.Entities.VkOrd;
using Domain.Extensions;
using Domain.VkOrdApi.Invoice;
using Microsoft.EntityFrameworkCore;
using WebApp.Models.Requests;
using WebApp.Repositories.Interfaces.VkOrd.Invoice;
using WebApp.Services.Interfaces;

namespace WebApp.Repositories.Implementations.VkOrd.Invoice;

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

        // Вызов VK Ord API
        var vkOrdClient = await _vkOrdClientFactory.CreateClient();
        
        await vkOrdClient.CreateFullInvoiceV3(
            externalId,
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
            ExternalId = externalId
        };

        invoice.Data = invoiceResponse;
        invoice.IsDraft = isDraft;
        invoice.ContractExternalId = request.ContractExternalId;
        invoice.UpdatedAt = DateTimeOffset.UtcNow;

        if (!isUpdate)
        {
            invoice.CreatedAt = DateTimeOffset.UtcNow;
        }

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
