using Domain.BrokenRules;
using Domain.Data;
using Domain.Entities;
using Domain.Entities.Enums;
using Domain.Entities.VkOrd;
using Domain.Extensions;
using Domain.Repositories.Interfaces.VkOrd.Contract;
using Domain.Repositories.Interfaces.VkOrd.Creative;
using Domain.Services.Interfaces;
using Domain.VkOrdApi.Creative;
using Microsoft.EntityFrameworkCore;

namespace Domain.Repositories.Implementations.VkOrd.Creative
{
    /// <summary>
    /// Репозиторий для получения креатива VK ORD API по external ID
    /// </summary>
    public class GetCreativeRepository : IGetCreativeRepository
    {
        private readonly IVkOrdApiClientFactory _vkOrdClientFactory;
        private readonly IGetContractRepository _getContractRepository;
        private readonly Func<AppDbContext> _contextFactory;

        public GetCreativeRepository(
            IVkOrdApiClientFactory vkOrdClientFactory,
            Func<AppDbContext> contextFactory,
            IGetContractRepository getContractRepository)
        {
            _vkOrdClientFactory = vkOrdClientFactory;
            _contextFactory = contextFactory;
            _getContractRepository = getContractRepository;
        }

        public async Task<VkOrdCreative> Get(string externalId, CancellationToken cancellationToken, bool noCache = false)
        {
            await using var context = _contextFactory();
            var vkOrdClient = await _vkOrdClientFactory.CreateClient();
            var vkOrdCredential = await _vkOrdClientFactory.GetVkOrdCredentialAsync();

            var data = await context.VkOrdCreatives.FirstOrDefaultAsync(
                AppDbContext.DefaultGetVkOrd<VkOrdCreative>(externalId, vkOrdCredential), cancellationToken);
            if (data == null || noCache)
            {
                var vkOrdData = await vkOrdClient.GetCreativeV3ByExternalId(externalId, cancellationToken);

                if (vkOrdData == null)
                {
                    throw BrokenRuleCodes.DataIsEmpty.AsExn();
                }

                data = await MapOperation(vkOrdData, data, vkOrdCredential, externalId, cancellationToken, context);
            }

            return data;
        }

        private async Task<VkOrdCreative> MapOperation(VkOrdApiCreativeV3Response vkOrdData, VkOrdCreative? data,
            ApiCredential vkOrdCredential, string externalId, CancellationToken cancellationToken, AppDbContext context)
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

            data ??= new VkOrdCreative
            {
                LogicalAccountId = vkOrdCredential.LogicalAccountId, ExternalId = externalId, ContractId = contract.Id
            };
            data.UpdateData(vkOrdData);

            if (data.IsNew())
            {
                data.CreativeContracts.Add(new VkOrdCreativeContract
                    { ContractId = contract.Id, CreativeId = data.Id });
                await context.VkOrdCreatives.AddAsync(data, cancellationToken);
            }
            else
            {
                context.VkOrdCreatives.Update(data);
            }

            await context.SaveChangesAsync(cancellationToken);
            return data;
        }

        private string GetCacheKey(string externalId, ApiCredential credential)
        {
            return $"vkord:{credential.LogicalAccountId}:{externalId}:{EntityType.Creative.GetDescription()}";
        }
    }
}