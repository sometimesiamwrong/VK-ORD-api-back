using Domain.VkOrdApi.Person;
using WebApp.Repositories.Interfaces.VkOrd.Counterparty;
using WebApp.Services.Interfaces;

namespace WebApp.Repositories.Implementations.VkOrd.Counterparty
{
    /// <summary>
    /// Репозиторий для создания контрагента VK ORD API по ИНН
    /// </summary>
    public class CreateCounterpartyRepository : ICreateCounterpartyRepository
    {
        private readonly IVkOrdApiClientFactory _vkOrdClientFactory;
        private readonly ILogger<CreateCounterpartyRepository> _logger;

        public CreateCounterpartyRepository(
            IVkOrdApiClientFactory vkOrdClientFactory,
            ILogger<CreateCounterpartyRepository> logger)
        {
            _vkOrdClientFactory = vkOrdClientFactory;
            _logger = logger;
        }

        public async Task Create(string externalId, VkOrdApiPersonResponse apiPerson, CancellationToken cancellationToken)
        {
            var vkOrdClient = await _vkOrdClientFactory.CreateClient();

            await vkOrdClient.CreateOrUpdatePerson(externalId, apiPerson, cancellationToken);
        }
    }
}
