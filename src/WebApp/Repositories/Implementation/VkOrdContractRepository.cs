using VkOrdApi.Services.Interfaces;
using WebApp.Models.Requests;
using WebApp.Models.Responses;
using WebApp.Repositories.Interfaces;
using Domain.Entities;
using WebApp.Services.Interfaces;
using VkOrdApi.Contract;

namespace WebApp.Repositories.Implementation
{
    /// <summary>
    /// Репозиторий для работы с контрактами VK ORD API
    /// </summary>
    public class VkOrdContractRepository : IVkOrdContractRepository
    {
        private readonly IVkOrdApiClientFactory _vkOrdClientFactory;
        private readonly ILogger<VkOrdContractRepository> _logger;
        private readonly IVkOrdApiClient _vkOrdClient;

        public VkOrdContractRepository(
            IVkOrdApiClientFactory vkOrdClientFactory,
            ILogger<VkOrdContractRepository> logger)
        {
            _vkOrdClient = vkOrdClientFactory.CreateClientAsync().GetAwaiter().GetResult();
            _logger = logger;
        }

        public Task CreateOrUpdateContractAsync(string externalId, VkOrdCreateUpdateContractRequest request, CancellationToken cancellationToken = default)
        {
           return _vkOrdClient.CreateOrUpdateContractAsync(
                externalId, 
                request, 
                cancellationToken: cancellationToken, 
                updateAdditionalContractsParties: null);
        }
    }
}
