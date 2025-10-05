using Domain.Entities;
using VkOrdApi.Creative;
using WebApp.Models.Responses;
using WebApp.Repositories.Interfaces.VkOrd.Creative;
using WebApp.Services.Interfaces;

namespace WebApp.Repositories.Implementation.VkOrd.Creative
{
    /// <summary>
    /// Репозиторий для получения креатива VK ORD API по external ID
    /// </summary>
    public class GetCreativeRepository : IGetCreativeRepository
    {
        private readonly IVkOrdApiClientFactory _vkOrdClientFactory;
        private readonly ILogger<GetCreativeRepository> _logger;

        public GetCreativeRepository(
            IVkOrdApiClientFactory vkOrdClientFactory,
            ILogger<GetCreativeRepository> logger)
        {
            _vkOrdClientFactory = vkOrdClientFactory;
            _logger = logger;
        }

        public async Task<VkOrdCreativeV3Response> GetCreative(string externalId, CancellationToken cancellationToken)
        {
            var vkOrdClient = await _vkOrdClientFactory.CreateClient();
            return await vkOrdClient.GetCreativeV3ByExternalId(externalId, cancellationToken);
        }
    }
}
