using WebApp.Models.Responses;
using WebApp.Repositories.Interfaces.VkOrd.Counterparty;
using WebApp.Services.Interfaces;

namespace WebApp.Repositories.Implementation.VkOrd.Counterparty
{
    /// <summary>
    /// Репозиторий для получения контрагента VK ORD API по ID
    /// </summary>
    public class GetCounterpartyByIdRepository : IGetCounterpartyByIdRepository
    {
        private readonly IVkOrdApiClientFactory _vkOrdClientFactory;
        private readonly ILogger<GetCounterpartyByIdRepository> _logger;

        public GetCounterpartyByIdRepository(
            IVkOrdApiClientFactory vkOrdClientFactory,
            ILogger<GetCounterpartyByIdRepository> logger)
        {
            _vkOrdClientFactory = vkOrdClientFactory;
            _logger = logger;
        }

        public async Task<GetCounterpartyResponse?> GetCounterpartyByIdAsync(string externalId, CancellationToken cancellationToken)
        {
            var vkOrdClient = await _vkOrdClientFactory.CreateClient();

            var person = await vkOrdClient.GetPerson(externalId, cancellationToken);

            if (person == null)
            {
                return null;
            }

            return new GetCounterpartyResponse
            {
                ExternalId = externalId,
                Data = person
            };
        }
    }
}
