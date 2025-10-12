using Domain;
using WebApp.Models.Responses;
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
        private readonly ILogger<GetPageCounterpartiesRepository> _logger;
        
        public GetPageCounterpartiesRepository(
            IVkOrdApiClientFactory vkOrdClientFactory,
            ILogger<GetPageCounterpartiesRepository> logger)
        {
            _vkOrdClientFactory = vkOrdClientFactory;
            _logger = logger;
        }

        public async Task<GetPageVkOrdResponse> Get(PageRequest pageRequest, CancellationToken cancellationToken)
        {
            var vkOrdClient = await _vkOrdClientFactory.CreateClient();

            var response = await vkOrdClient.GetPersons(pageRequest, cancellationToken);

            return new GetPageVkOrdResponse
            {
                ExternalIds = response.ExternalIds,
                TotalItemsCount = response.TotalItemsCount,
                Limit = response.Limit
            };
        }
    }
}
