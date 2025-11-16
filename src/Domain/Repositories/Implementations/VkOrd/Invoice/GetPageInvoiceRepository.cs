using Domain.Data;
using Domain.Extensions;
using Domain.Repositories.Interfaces.VkOrd.Invoice;
using Domain.Services.Interfaces;
using Domain.VkOrdApi.Invoice;
using Microsoft.EntityFrameworkCore;

namespace Domain.Repositories.Implementations.VkOrd.Invoice;

/// <summary>
/// Репозиторий для получения списка актов VK ORD API с пагинацией
/// </summary>
public class GetPageInvoiceRepository : IGetPageInvoiceRepository
{
    private readonly IVkOrdApiClientFactory _vkOrdClientFactory;
    private readonly Func<AppDbContext> _contextFactory;
    private readonly ICacheService _cacheService;
    private readonly ILogger<GetPageInvoiceRepository> _logger;

    public GetPageInvoiceRepository(
        IVkOrdApiClientFactory vkOrdClientFactory,
        Func<AppDbContext> contextFactory,
        ICacheService cacheService,
        ILogger<GetPageInvoiceRepository> logger)
    {
        _vkOrdClientFactory = vkOrdClientFactory;
        _contextFactory = contextFactory;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<VkOrdApiInvoiceListResponse> Get(PageRequest pageRequest, CancellationToken cancellationToken, List<string>? externalIds = null)
    {
        await using var context = _contextFactory();
        var vkOrdClient = await _vkOrdClientFactory.CreateClient();
        var apiCredential = await _vkOrdClientFactory.GetVkOrdCredentialAsync();

        var query = context.VkOrdInvoices.AsQueryable();
        if (externalIds != null)
        {
            query = query
                .Include(x => x.Contract)
                    .ThenInclude(c => c.ContractParties)
                        .ThenInclude(cp => cp.Counterparty)
                .Where(x => x.Contract != null && x.Contract.ContractParties.Any(cp => externalIds.Contains(cp.Counterparty.ExternalId)));
        }

        query = query
            .Where(x => x.LogicalAccountId == apiCredential.LogicalAccountId)
            .Skip((pageRequest.Page - 1) * pageRequest.Limit)
            .Take(pageRequest.Limit)
            .OrderBy(x => x.UpdatedAt);

        var data = await query.ToListAsync(cancellationToken);
        var totalItemsCount = await query.CountAsync(cancellationToken);

        if(!data.IsNullOrEmpty())
        {
            return new VkOrdApiInvoiceListResponse
            {
                ExternalIds = data.Select(x => x.ExternalId).ToList(),
                TotalItemsCount = totalItemsCount,
                Limit = pageRequest.Limit
            };
        }

        return new VkOrdApiInvoiceListResponse
        {
            ExternalIds = new List<string>(),
            TotalItemsCount = 0,
            Limit = pageRequest.Limit
        };
    }
}
