using Domain.BrokenRules;
using Domain.Data;
using Domain.Entities;
using Domain.Entities.Enums;
using Domain.Entities.VkOrd;
using Domain.Extensions;
using Domain.VkOrdApi.Invoice;
using Microsoft.EntityFrameworkCore;
using WebApp.Repositories.Interfaces.VkOrd.Invoice;
using WebApp.Services.Interfaces;

namespace WebApp.Repositories.Implementations.VkOrd.Invoice;

/// <summary>
/// Репозиторий для получения акта VK ORD API
/// </summary>
public class GetInvoiceRepository : IGetInvoiceRepository
{
    private readonly IVkOrdApiClientFactory _vkOrdClientFactory;
    private readonly ICacheService _cacheService;
    private readonly AppDbContext _context;
    private readonly ILogger<GetInvoiceRepository> _logger;

    public GetInvoiceRepository(
        IVkOrdApiClientFactory vkOrdClientFactory,
        ICacheService cacheService,
        AppDbContext context,
        ILogger<GetInvoiceRepository> logger)
    {
        _vkOrdClientFactory = vkOrdClientFactory;
        _cacheService = cacheService;
        _context = context;
        _logger = logger;
    }

    public async Task<VkOrdInvoice> Get(string externalId, CancellationToken cancellationToken)
    {
        var vkOrdCredential = await _vkOrdClientFactory.GetVkOrdCredentialAsync();
        var cacheKey = GetCacheKey(externalId, vkOrdCredential);

        // Получаем данные из кэша
        var data = await _cacheService.Get<VkOrdInvoice>(cacheKey, cancellationToken);

        // Если данных нет в кэше, получаем из БД
        if (data == null)
        {
            data = await _context.VkOrdInvoices
                .Include(x => x.Contract)
                    .ThenInclude(x => x.CreativeContracts)
                        .ThenInclude(x => x.Creative)
                .FirstOrDefaultAsync(
                    AppDbContext.DefaultGetVkOrd<VkOrdInvoice>(externalId, vkOrdCredential),
                    cancellationToken);

            if (data == null)
            {
                // Получаем данные из API
                var vkOrdData = await GetByApi(externalId, cancellationToken);

                if (vkOrdData == null)
                {
                    throw BrokenRuleCodes.DataIsEmpty.AsExn();
                }

                // Мапим данные
                data = MapOperation(vkOrdData, vkOrdCredential, externalId);

                // Сохраняем данные в базу
                await SaveToDatabase(data, cancellationToken);
            }

            // Сохраняем данные в кэш
            await _cacheService.Save(cacheKey, data, cancellationToken);
        }

        return data;
    }

    private VkOrdInvoice MapOperation(VkOrdApiFullInvoiceResponse response, ApiCredential vkOrdCredential, string externalId)
    {
        var data = new VkOrdInvoice
        {
            LogicalAccountId = vkOrdCredential.LogicalAccountId,
            ExternalId = externalId
        };

        data.UpdateData(response);
        data.ContractExternalId = response.ContractExternalId;

        return data;
    }

    private async Task SaveToDatabase(VkOrdInvoice data, CancellationToken cancellationToken)
    {
        if (data.IsNew())
        {
            await _context.VkOrdInvoices.AddAsync(data, cancellationToken);
        }
        else
        {
            _context.VkOrdInvoices.Update(data);
        }

        await _context.SaveChangesAsync(cancellationToken);
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
