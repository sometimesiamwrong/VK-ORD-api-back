using Domain.BrokenRules;
using Domain.Data;
using Domain.Entities;
using Domain.Entities.Enums;
using Domain.Entities.VkOrd;
using Domain.Extensions;
using Domain.Repositories.Interfaces.VkOrd.Invoice;
using Domain.Services.Interfaces;
using Domain.VkOrdApi.Invoice;
using Microsoft.EntityFrameworkCore;

namespace Domain.Repositories.Implementations.VkOrd.Invoice;

/// <summary>
/// Репозиторий для получения акта VK ORD API
/// </summary>
public class GetInvoiceRepository : IGetInvoiceRepository
{
    private readonly IVkOrdApiClientFactory _vkOrdClientFactory;
    private readonly ICacheService _cacheService;
    private readonly Func<AppDbContext> _contextFactory;
    private readonly ILogger<GetInvoiceRepository> _logger;

    public GetInvoiceRepository(
        IVkOrdApiClientFactory vkOrdClientFactory,
        ICacheService cacheService,
        Func<AppDbContext> contextFactory,
        ILogger<GetInvoiceRepository> logger)
    {
        _vkOrdClientFactory = vkOrdClientFactory;
        _cacheService = cacheService;
        _contextFactory = contextFactory;
        _logger = logger;
    }

    public async Task<VkOrdInvoice> Get(string externalId, CancellationToken cancellationToken, bool noCache = false)
    {
        await using var context = _contextFactory();
        var vkOrdCredential = await _vkOrdClientFactory.GetVkOrdCredentialAsync();

        var data = await context.VkOrdInvoices
            .Include(x => x.Contract)
            .ThenInclude(x => x.CreativeContracts)
            .ThenInclude(x => x.Creative)
            .FirstOrDefaultAsync(
                AppDbContext.DefaultGetVkOrd<VkOrdInvoice>(externalId, vkOrdCredential),
                cancellationToken);

        if (data == null || data.IsExpired() || noCache)
        {
            // Получаем данные из API
            var vkOrdData = await GetByApi(externalId, cancellationToken);

            if (vkOrdData == null)
            {
                throw BrokenRuleCodes.DataIsEmpty.AsExn();
            }

            // Мапим данные
            data = MapOperation(data, vkOrdData, vkOrdCredential, externalId);

            // Сохраняем данные в базу
            await SaveToDatabase(data, cancellationToken, context);
        }

        return data;
    }

    private VkOrdInvoice MapOperation(VkOrdInvoice? data, VkOrdApiFullInvoiceResponse response, ApiCredential vkOrdCredential,
        string externalId)
    {
        data ??= new VkOrdInvoice
        {
            LogicalAccountId = vkOrdCredential.LogicalAccountId,
            ExternalId = externalId
        };

        data.UpdateData(response);
        data.ContractExternalId = response.ContractExternalId;

        return data;
    }

    private async Task SaveToDatabase(VkOrdInvoice data, CancellationToken cancellationToken, AppDbContext context)
    {
        if (data.IsNew())
        {
            await context.VkOrdInvoices.AddAsync(data, cancellationToken);
        }
        else
        {
            context.VkOrdInvoices.Update(data);
        }

        await context.SaveChangesAsync(cancellationToken);
    }

    private async Task<VkOrdApiFullInvoiceResponse?> GetByApi(string externalId, CancellationToken cancellationToken)
    {
        var vkOrdClient = await _vkOrdClientFactory.CreateClient();

        try
        {
            var invoice = await vkOrdClient.GetFullInvoiceV3(externalId, cancellationToken);
            return invoice;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting invoice {externalId} from VK ORD API");
            return null;
        }
    }

    private string GetCacheKey(string externalId, ApiCredential apiCredential)
    {
        return $"vkord:{apiCredential.LogicalAccountId}:{externalId}:{EntityType.Invoice.GetDescription()}";
    }
}