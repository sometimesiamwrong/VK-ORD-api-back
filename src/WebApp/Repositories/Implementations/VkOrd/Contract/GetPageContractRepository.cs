using Domain;
using WebApp.Models.Responses;
using WebApp.Repositories.Interfaces.VkOrd.Contract;
using WebApp.Services.Interfaces;

namespace WebApp.Repositories.Implementations.VkOrd.Contract
{
    /// <summary>
    /// Репозиторий для получения списка контрактов VK ORD API
    /// </summary>
    public class GetPageContractRepository : IGetPageContractRepository
    {
        private readonly IVkOrdApiClientFactory _vkOrdClientFactory;
        private readonly ILogger<GetPageContractRepository> _logger;
        
        public GetPageContractRepository(
            IVkOrdApiClientFactory vkOrdClientFactory,
            ILogger<GetPageContractRepository> logger)
        {
            _vkOrdClientFactory = vkOrdClientFactory;
            _logger = logger;
        }

        public async Task<GetPageVkOrdResponse> Get(PageRequest pageRequest, CancellationToken cancellationToken)
        {
            var vkOrdClient = await _vkOrdClientFactory.CreateClient();

            var response = await vkOrdClient.GetContracts(pageRequest, cancellationToken);

            return new GetPageVkOrdResponse
            {
                ExternalIds = response.ExternalIds,
                TotalItemsCount = response.TotalItemsCount,
                Limit = response.Limit
            };
        }
    }
}
