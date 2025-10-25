using Domain.BrokenRules;
using Domain.Extensions;
using Domain.VkOrdApi;
using Domain.VkOrdApi.Contract;
using WebApp.Repositories.Interfaces.VkOrd.Contract;
using WebApp.Services.Interfaces;

namespace WebApp.Repositories.Implementations.VkOrd.Contract
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
