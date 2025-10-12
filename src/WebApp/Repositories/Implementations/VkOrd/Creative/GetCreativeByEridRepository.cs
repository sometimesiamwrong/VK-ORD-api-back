using Domain.VkOrdApi.Creative;
using WebApp.Repositories.Interfaces.VkOrd.Creative;
using WebApp.Services.Interfaces;

namespace WebApp.Repositories.Implementations.VkOrd.Creative
{
    /// <summary>
    /// Репозиторий для получения креатива VK ORD API по ERID
    /// </summary>
    public class GetCreativeByEridRepository : IGetCreativeByEridRepository
    {
        private readonly IVkOrdApiClientFactory _vkOrdClientFactory;
        private readonly ILogger<GetCreativeByEridRepository> _logger;

        public GetCreativeByEridRepository(
            IVkOrdApiClientFactory vkOrdClientFactory,
            ILogger<GetCreativeByEridRepository> logger)
        {
            _vkOrdClientFactory = vkOrdClientFactory;
            _logger = logger;
        }

        public async Task<VkOrdApiCreativeV3Response> GetCreativeByErid(string erid, CancellationToken cancellationToken)
        {
            var vkOrdClient = await _vkOrdClientFactory.CreateClient();
            return await vkOrdClient.GetCreativeV3ByErid(erid, cancellationToken);
        }
    }
}
