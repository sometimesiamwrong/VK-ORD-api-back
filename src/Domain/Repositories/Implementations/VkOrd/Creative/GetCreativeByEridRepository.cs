using Domain.BrokenRules;
using Domain.Data;
using Domain.Entities;
using Domain.Entities.Enums;
using Domain.Entities.VkOrd;
using Domain.Extensions;
using Domain.Repositories.Interfaces.VkOrd.Creative;
using Domain.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Domain.Repositories.Implementations.VkOrd.Creative
{
    /// <summary>
    /// Репозиторий для получения креатива VK ORD API по ERID
    /// </summary>
    public class GetCreativeByEridRepository : IGetCreativeByEridRepository
    {
        private readonly IVkOrdApiClientFactory _vkOrdClientFactory;
        private readonly ILogger<GetCreativeByEridRepository> _logger;
        private readonly ICacheService _cacheService;
        private readonly Func<AppDbContext> _contextFactory;
        public GetCreativeByEridRepository(
            IVkOrdApiClientFactory vkOrdClientFactory,
            ICacheService cacheService,
            Func<AppDbContext> contextFactory,
            ILogger<GetCreativeByEridRepository> logger)
        {
            _vkOrdClientFactory = vkOrdClientFactory;
            _logger = logger;
            _cacheService = cacheService;
            _contextFactory = contextFactory;
        }

        public async Task<VkOrdCreative> GetCreativeByErid(string erid, CancellationToken cancellationToken)
        {
            await using var context = _contextFactory();
            var vkOrdClient = await _vkOrdClientFactory.CreateClient();
            var vkOrdCredential = await _vkOrdClientFactory.GetVkOrdCredentialAsync();
            var cacheKey = GetCacheKey(erid, vkOrdCredential);
            var data = await _cacheService.Get<VkOrdCreative>(cacheKey, cancellationToken);
            if (data == null || data.IsExpired())
            {
                data = await context.VkOrdCreatives.FirstOrDefaultAsync(
                    c => c.LogicalAccountId == vkOrdCredential.LogicalAccountId && c.Data.Erid == erid,
                    cancellationToken);
                if (data == null)
                {
                    var vkOrdData = await vkOrdClient.GetCreativeV3ByErid(erid, cancellationToken);
                    if (vkOrdData == null)
                    {
                        throw BrokenRuleCodes.DataIsEmpty.AsExn();
                    }
                    data = new VkOrdCreative { LogicalAccountId = vkOrdCredential.LogicalAccountId, ExternalId = vkOrdData.Erid };
                    data.UpdateData(vkOrdData);
                    await context.VkOrdCreatives.AddAsync(data, cancellationToken);
                    await context.SaveChangesAsync(cancellationToken);
                }
                await _cacheService.Save(cacheKey, data, cancellationToken);
            }
            return data;
        }

        private string GetCacheKey(string erid, ApiCredential credential)
        {
            return $"vkord:{credential.LogicalAccountId}:{erid}:{EntityType.Creative.GetDescription()}";
        }
    }
}
