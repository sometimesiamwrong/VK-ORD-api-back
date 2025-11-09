using Domain;
using Domain.Data;
using Domain.Entities;
using Domain.Entities.Enums;
using Domain.Extensions;
using Microsoft.EntityFrameworkCore;
using WebApp.Models.Responses;
using WebApp.Repositories.Interfaces;
using WebApp.Repositories.Interfaces.VkOrd.Counterparty;
using WebApp.Services.Interfaces;

namespace WebApp.Repositories.Implementations.VkOrd.Counterparty
{
    /// <summary>
    /// Репозиторий для получения списка контрагентов VK ORD API
    /// </summary>
    public class GetPageCounterpartiesRepository : IGetPageCounterpartiesRepository
    {
        private readonly IVkOrdApiClientFactory _vkOrdClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IGetCounterpartyByIdRepository _getCounterpartyByIdRepository;
        private readonly AppDbContext _context; 
        
        public GetPageCounterpartiesRepository(
            IVkOrdApiClientFactory vkOrdClientFactory,
            IHttpContextAccessor httpContextAccessor,
            AppDbContext context,
            IGetCounterpartyByIdRepository getCounterpartyByIdRepository)
        {
            _vkOrdClientFactory = vkOrdClientFactory;
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _getCounterpartyByIdRepository = getCounterpartyByIdRepository;
        }

        public async Task<GetPageVkOrdResponse> Get(PageRequest pageRequest, CancellationToken cancellationToken)
        {
            var noCache = _httpContextAccessor.GetNoCacheHeader();
            var vkOrdCredential = await _vkOrdClientFactory.GetVkOrdCredentialAsync();
            var vkOrdClient = await _vkOrdClientFactory.CreateClient();

            var now = DateTimeOffset.UtcNow;
            var query = _context.VkOrdCounterparties
                .Where(x=> x.LogicalAccountId == vkOrdCredential.LogicalAccountId);

            var data = new GetPageVkOrdResponse
            {
                ExternalIds = await query
                    .OrderBy(x=>x.UpdatedAt)
                    .Skip(pageRequest.Offset)
                    .Take(pageRequest.Limit)
                    .Select(x=>x.ExternalId)
                    .ToListAsync(cancellationToken),
                TotalItemsCount = await query.CountAsync(cancellationToken),
                Limit = pageRequest.Limit
            };

            if(data.ExternalIds.IsNullOrEmpty() || noCache)
            {
                var vkOrdResponse = await vkOrdClient.GetPersons(pageRequest, cancellationToken);
                data.ExternalIds = vkOrdResponse.ExternalIds;
                data.TotalItemsCount = vkOrdResponse.TotalItemsCount;
                data.Limit = vkOrdResponse.Limit;

                foreach (var externalId in data.ExternalIds)
                {
                    await _getCounterpartyByIdRepository.Get(externalId, cancellationToken);
                }
            }

            return data;
        }

        private string GetCacheKey(PageRequest pageRequest, ApiCredential vkOrdCredential)
        {
            return $"vkord:{vkOrdCredential.LogicalAccountId}:{pageRequest.Offset}:{pageRequest.Limit}:{EntityType.Counterparty.GetDescription()}";
        }
    }
}
