using Domain.VkOrdApi.Creative;
using WebApp.Repositories.Interfaces.VkOrd.Creative;
using WebApp.Services.Interfaces;

namespace WebApp.Repositories.Implementations.VkOrd.Creative
{
    /// <summary>
    /// Репозиторий для создания креатива VK ORD API
    /// </summary>
    public class CreateCreativeRepository : ICreateCreativeRepository
    {
        private readonly IVkOrdApiClientFactory _vkOrdClientFactory;
        private readonly ILogger<CreateCreativeRepository> _logger;

        public CreateCreativeRepository(
            IVkOrdApiClientFactory vkOrdClientFactory,
            ILogger<CreateCreativeRepository> logger)
        {
            _vkOrdClientFactory = vkOrdClientFactory;
            _logger = logger;
        }

        public async Task<VkOrdApiCreativeV3RequestResponse> CreateCreative(string externalId, VkOrdApiCreativeV3Request request, CancellationToken cancellationToken)
        {
            var vkOrdClient = await _vkOrdClientFactory.CreateClient();

            var response = await vkOrdClient.CreateOrUpdateCreativeV3(externalId, request, cancellationToken);

            return new VkOrdApiCreativeV3RequestResponse
            {
                Erid = response.Erid,
            };
        }
    }
}
