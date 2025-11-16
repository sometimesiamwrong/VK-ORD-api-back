using Domain.Data;
using Domain.Entities;
using Domain.Entities.Enums;
using Domain.Entities.VkOrd;
using Domain.Extensions;
using Domain.Repositories.Interfaces.VkOrd.Contract;
using Domain.Repositories.Interfaces.VkOrd.Counterparty;
using Domain.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Domain.Repositories.Implementations.VkOrd.Contract
{
    public class GetContractsByCounterpartyRepository : IGetContractsByCounterpartyRepository
    {
        private readonly IVkOrdApiClientFactory _vkOrdClientFactory;
        private readonly IGetCounterpartyByIdRepository _getCounterpartyByIdRepository;
        private readonly ICacheService _cacheService;
        private readonly Func<AppDbContext> _contextFactory;

        public GetContractsByCounterpartyRepository(
            IVkOrdApiClientFactory vkOrdClientFactory, 
            IGetCounterpartyByIdRepository getCounterpartyByIdRepository, 
            ICacheService cacheService, 
            Func<AppDbContext> contextFactory)
        {
            _vkOrdClientFactory = vkOrdClientFactory;
            _getCounterpartyByIdRepository = getCounterpartyByIdRepository;
            _cacheService = cacheService;
            _contextFactory = contextFactory;
        }

        public async Task<List<VkOrdContract>> Get(string counterpartyExternalId, CancellationToken cancellationToken)
        {
            await using var context = _contextFactory();
            var vkOrdCredential = await _vkOrdClientFactory.GetVkOrdCredentialAsync();
            var cacheKey = GetCacheKey(counterpartyExternalId, vkOrdCredential);

            var data = await _cacheService.Get<List<VkOrdContract>>(cacheKey, cancellationToken);

            if (data == null)
            {
                var counterparty = await _getCounterpartyByIdRepository.Get(counterpartyExternalId, cancellationToken);
                data = await context.VkOrdContracts
                    .Where(
                        x=>x.LogicalAccountId == vkOrdCredential.LogicalAccountId &&
                        x.ContractParties.Any(y=>y.CounterpartyId == counterparty.Id)
                    )
                    .Include(x=>x.ContractParties)
                    .OrderBy(x=>x.UpdatedAt)
                    .ToListAsync(cancellationToken);

                await _cacheService.Save(cacheKey, data, cancellationToken);
            }

            return data;
        }

        private static string GetCacheKey(string counterpartyExternalId, ApiCredential vkOrdCredential)
        {
            return $"vkord:GetContractsByCounterpartyRepository:{vkOrdCredential.LogicalAccountId}:{counterpartyExternalId}:{EntityType.Contract.GetDescription()}";
        }
    }
}