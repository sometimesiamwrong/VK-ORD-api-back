using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using WebApp.Models.Responses;
using WebApp.Repositories.Interfaces.VkOrd.Counterparty;
using WebApp.Services.Interfaces;

namespace WebApp.Repositories.Implementation.VkOrd.Counterparty
{
    /// <summary>
    /// Репозиторий для получения контрагента VK ORD API по ID с кэшированием
    /// </summary>
    public class GetCounterpartyByIdCacheRepository : IGetCounterpartyByIdRepository
    {
        private readonly IGetCounterpartyByIdRepository _getCounterpartyByIdRepository;
        private readonly IDistributedCache _cacheRepository;

        public GetCounterpartyByIdCacheRepository(IGetCounterpartyByIdRepository getCounterpartyByIdRepository, IDistributedCache cacheRepository)
        {
            _getCounterpartyByIdRepository = getCounterpartyByIdRepository;
            _cacheRepository = cacheRepository;
        }

        public async Task<GetCounterpartyResponse?> GetCounterpartyByIdAsync(string externalId, CancellationToken cancellationToken)
        {
            var cached = await GetCachedCounterpartyAsync(externalId);
            if (cached != null)
            {
                return cached;
            }

            var result = await _getCounterpartyByIdRepository.GetCounterpartyByIdAsync(externalId, cancellationToken);
            if (result != null)
            {
                await SetCachedCounterpartyAsync(externalId, result);
            }
            return result;
        }

        // GetCounterpartyResponse кэш
        public async Task<GetCounterpartyResponse?> GetCachedCounterpartyAsync(string externalId)
        {
            var cacheKey = $"counterparty_{externalId}";
            var cachedJson = await _cacheRepository.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedJson))
            {
                return JsonSerializer.Deserialize<GetCounterpartyResponse>(cachedJson);
            }
            return null;
        }

        public async Task SetCachedCounterpartyAsync(string externalId, GetCounterpartyResponse counterpartyResponse)
        {
            var cacheKey = $"counterparty_{externalId}";
            var json = JsonSerializer.Serialize(counterpartyResponse);
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
            };
            await _cacheRepository.SetStringAsync(cacheKey, json, options);
        }
    }

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
