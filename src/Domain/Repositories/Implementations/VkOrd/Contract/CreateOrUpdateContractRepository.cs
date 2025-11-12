using Domain.Repositories.Interfaces.VkOrd.Contract;
using Domain.Services.Interfaces;
using Domain.VkOrdApi.Contract;

namespace Domain.Repositories.Implementations.VkOrd.Contract
{
    /// <summary>
    /// Репозиторий для создания/обновления контрактов VK ORD API
    /// </summary>
    public class CreateOrUpdateContractRepository : ICreateOrUpdateContractRepository
    {
        private readonly IVkOrdApiClientFactory _vkOrdClientFactory;
        private readonly IGetContractRepository _getContractRepository;
        private readonly ILogger<CreateOrUpdateContractRepository> _logger;

        public CreateOrUpdateContractRepository(
            IVkOrdApiClientFactory vkOrdClientFactory,
            IGetContractRepository getContractRepository,
            ILogger<CreateOrUpdateContractRepository> logger)
        {
            _vkOrdClientFactory = vkOrdClientFactory;
            _getContractRepository = getContractRepository;
            _logger = logger;
        }

        public async Task CreateOrUpdateContract(string externalId, VkOrdApiCreateUpdateContractRequest request, CancellationToken cancellationToken)
        { 
            var vkOrdClient = await _vkOrdClientFactory.CreateClient();

            await vkOrdClient.CreateOrUpdateContract(externalId, request, cancellationToken: cancellationToken, updateAdditionalContractsParties: null);

            await _getContractRepository.Get(externalId, cancellationToken);
        }
    }
}
