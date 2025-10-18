using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Data;
using Domain.Entities;
using Domain.Entities.Enums;
using Domain.Entities.VkOrd;
using Domain.Extensions;
using Microsoft.EntityFrameworkCore;
using WebApp.Repositories.Interfaces.VkOrd.Contract;
using WebApp.Repositories.Interfaces.VkOrd.Counterparty;
using WebApp.Services.Interfaces;

namespace WebApp.Repositories.Implementations.VkOrd.Contract
{
    public class GetContractsByCounterpartyRepository : IGetContractsByCounterpartyRepository
    {
        private readonly IVkOrdApiClientFactory _vkOrdClientFactory;
        private readonly IGetCounterpartyByIdRepository _getCounterpartyByIdRepository;
        private readonly ICacheService _cacheService;
        private readonly AppDbContext _context;

        public GetContractsByCounterpartyRepository(
            IVkOrdApiClientFactory vkOrdClientFactory, 
            IGetCounterpartyByIdRepository getCounterpartyByIdRepository, 
            ICacheService cacheService, 
            AppDbContext context)
        {
            _vkOrdClientFactory = vkOrdClientFactory;
            _getCounterpartyByIdRepository = getCounterpartyByIdRepository;
            _cacheService = cacheService;
            _context = context;
        }

        public async Task<List<VkOrdContract>> Get(string counterpartyExternalId, CancellationToken cancellationToken)
        {
            var vkOrdCredential = await _vkOrdClientFactory.GetVkOrdCredentialAsync();
            var cacheKey = GetCacheKey(counterpartyExternalId, vkOrdCredential);

            var data = await _cacheService.Get<List<VkOrdContract>>(cacheKey, cancellationToken);

            if (data == null)
            {
                var counterparty = await _getCounterpartyByIdRepository.Get(counterpartyExternalId, cancellationToken);
                data = await _context.VkOrdContracts
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

        private string GetCacheKey(string counterpartyExternalId, ApiCredential vkOrdCredential)
        {
            return $"vkord:GetContractsByCounterpartyRepository:{vkOrdCredential.LogicalAccountId}:{counterpartyExternalId}:{EntityType.Contract.GetDescription()}";
        }
    }
}