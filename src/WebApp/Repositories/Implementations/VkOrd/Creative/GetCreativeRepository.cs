using Domain.BrokenRules;
using Domain.Data;
using Domain.Entities;
using Domain.Entities.Enums;
using Domain.Entities.VkOrd;
using Domain.Extensions;
using Domain.VkOrdApi.Creative;
using Microsoft.EntityFrameworkCore;
using WebApp.Repositories.Interfaces.VkOrd.Contract;
using WebApp.Repositories.Interfaces.VkOrd.Creative;
using WebApp.Services.Interfaces;

namespace WebApp.Repositories.Implementations.VkOrd.Creative
{
    /// <summary>
    /// Репозиторий для получения креатива VK ORD API по external ID
    /// </summary>
    public class GetCreativeRepository : IGetCreativeRepository
    {
        private readonly IVkOrdApiClientFactory _vkOrdClientFactory;
        private readonly IGetContractRepository _getContractRepository;
        private readonly ICacheService _cacheService;
        private readonly AppDbContext _context;
        private readonly ILogger<GetCreativeRepository> _logger;

        public GetCreativeRepository(
            IVkOrdApiClientFactory vkOrdClientFactory,
            ICacheService cacheService,
            AppDbContext context,
            ILogger<GetCreativeRepository> logger,
            IGetContractRepository getContractRepository)
        {
            _vkOrdClientFactory = vkOrdClientFactory;
            _cacheService = cacheService;
            _context = context;
            _logger = logger;
            _getContractRepository = getContractRepository;
        }   

        public async Task<VkOrdCreative> Get(string externalId, CancellationToken cancellationToken)
        {
            var vkOrdClient = await _vkOrdClientFactory.CreateClient();
            var vkOrdCredential = await _vkOrdClientFactory.GetVkOrdCredentialAsync();
            var cacheKey = GetCacheKey(externalId, vkOrdCredential);

            var data = await _cacheService.Get<VkOrdCreative>(cacheKey, cancellationToken);
            if (data == null || data.IsExpired())
            {
                data = await _context.VkOrdCreatives.FirstOrDefaultAsync(AppDbContext.DefaultGetVkOrd<VkOrdCreative>(externalId, vkOrdCredential), cancellationToken);
                if (data == null)
                {
                    var vkOrdData = await vkOrdClient.GetCreativeV3ByExternalId(externalId, cancellationToken);

                    if (vkOrdData == null)
                    {
                        throw BrokenRuleCodes.DataIsEmpty.AsExn();
                    }

                    data = await MapOperation(vkOrdData, data, vkOrdCredential, externalId, cancellationToken);
                }

                await _cacheService.Save(cacheKey, data, cancellationToken);
            }

            return data;
        }

        private async Task<VkOrdCreative> MapOperation(VkOrdApiCreativeV3Response vkOrdData, VkOrdCreative data, ApiCredential vkOrdCredential, string externalId, CancellationToken cancellationToken)
        {
            var contractExternalId = vkOrdData.ContractExternalIds?.FirstOrDefault();
            if (contractExternalId == null)
            {
                throw BrokenRuleCodes.DataIsEmpty.AsExn();
            }
            var contract = await _getContractRepository.Get(contractExternalId, cancellationToken);
            if (contract == null)
            {
                throw BrokenRuleCodes.DataIsEmpty.AsExn();
            }

            data = new VkOrdCreative { LogicalAccountId = vkOrdCredential.LogicalAccountId, ExternalId = externalId, ContractId = contract.Id };
            data.UpdateData(vkOrdData);

            data.CreativeContracts.Add(new VkOrdCreativeContract { ContractId = contract.Id, CreativeId = data.Id });
            await _context.VkOrdCreatives.AddAsync(data, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return data;
        }

        private string GetCacheKey(string externalId, ApiCredential credential)
        {
            return $"vkord:{credential.LogicalAccountId}:{externalId}:{EntityType.Creative.GetDescription()}";
        }
    }
}
