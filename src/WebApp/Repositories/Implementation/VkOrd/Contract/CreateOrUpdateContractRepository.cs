using VkOrdApi.Contract;
using WebApp.Repositories.Interfaces.VkOrd.Contract;
using WebApp.Services.Interfaces;

namespace WebApp.Repositories.Implementation.VkOrd.Contract
{
    /// <summary>
    /// Репозиторий для создания/обновления контрактов VK ORD API
    /// </summary>
    public class CreateOrUpdateContractRepository : ICreateOrUpdateContractRepository
    {
        private readonly IVkOrdApiClientFactory _vkOrdClientFactory;
        private readonly ILogger<CreateOrUpdateContractRepository> _logger;
        private readonly IVkOrdApiClient _vkOrdClient;

        public CreateOrUpdateContractRepository(
            IVkOrdApiClientFactory vkOrdClientFactory,
            ILogger<CreateOrUpdateContractRepository> logger)
        {
            _vkOrdClient = vkOrdClientFactory.CreateClient().GetAwaiter().GetResult();
            _logger = logger;
        }

        public Task CreateOrUpdateContract(string externalId, VkOrdCreateUpdateContractRequest request, CancellationToken cancellationToken)
        {
           return _vkOrdClient.CreateOrUpdateContract(
                externalId, 
                request, 
                cancellationToken: cancellationToken, 
                updateAdditionalContractsParties: null);
        }
    }
}
