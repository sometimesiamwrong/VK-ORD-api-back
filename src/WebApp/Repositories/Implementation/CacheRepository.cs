using System.Text.Json;
using Domain.Entities;
using Microsoft.Extensions.Caching.Distributed;
using VkOrdApi.Person;
using WebApp.Models.Responses;
using WebApp.Repositories.Interfaces;

namespace WebApp.Repositories.Implementation
{
    /// <summary>
    /// Репозиторий для работы с кэшем
    /// </summary>
    public class CacheRepository : ICacheRepository
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<CacheRepository> _logger;

        public CacheRepository(IDistributedCache cache, ILogger<CacheRepository> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        // DaData кэш
        public async Task<DaDataPartyShortResponse?> GetCachedPartyByInnAsync(string inn)
        {
            var cacheKey = $"dadata:party:{inn}";
            var cachedJson = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedJson))
            {
                return JsonSerializer.Deserialize<DaDataPartyShortResponse>(cachedJson);
            }
            return null;
        }

        public async Task SetCachedPartyByInnAsync(string inn, DaDataPartyShortResponse party)
        {
            var cacheKey = $"dadata:party:{inn}";
            var json = JsonSerializer.Serialize(party);
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
            };
            await _cache.SetStringAsync(cacheKey, json, options);
        }

        // Контракт кэш
        public async Task<string?> GetCachedContractFlagAsync(string externalId)
        {
            var cacheKey = $"contract_{externalId}";
            return await _cache.GetStringAsync(cacheKey);
        }

        public async Task SetCachedContractFlagAsync(string externalId)
        {
            var cacheKey = $"contract_{externalId}";
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
            };
            await _cache.SetStringAsync(cacheKey, "1", options);
        }

        // Креатив кэш
        public async Task<string?> GetCachedCreativeAsync(string externalId)
        {
            var cacheKey = $"creative_{externalId}";
            return await _cache.GetStringAsync(cacheKey);
        }

        public async Task SetCachedCreativeAsync(string externalId, CreateCreativeResponse response)
        {
            var cacheKey = $"creative_{externalId}";
            var json = JsonSerializer.Serialize(response);
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
            };
            await _cache.SetStringAsync(cacheKey, json, options);
        }

        // Контрагент кэш
        public async Task<VkOrdPersonResponse?> GetCachedCounterpartyAsync(Guid userId, string? environment, string externalId)
        {
            var cacheKey = $"person_{userId}_{environment ?? "default"}_{externalId}";
            var cachedJson = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedJson))
            {
                return JsonSerializer.Deserialize<VkOrdPersonResponse>(cachedJson);
            }
            return null;
        }

        public async Task SetCachedCounterpartyAsync(Guid userId, string? environment, string externalId, VkOrdPersonResponse personResponse)
        {
            var cacheKey = $"person_{userId}_{environment ?? "default"}_{externalId}";
            var json = JsonSerializer.Serialize(personResponse);
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
            };
            await _cache.SetStringAsync(cacheKey, json, options);
        }

        // Медиа кэш
        public async Task<string?> GetCachedMediaAsync(string externalId)
        {
            var cacheKey = $"media_{externalId}";
            return await _cache.GetStringAsync(cacheKey);
        }

        public async Task SetCachedMediaAsync(string externalId, UploadMediaResponse response)
        {
            var cacheKey = $"media_{externalId}";
            var json = JsonSerializer.Serialize(response);
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
            };
            await _cache.SetStringAsync(cacheKey, json, options);
        }

        // Удаление из кэша
        public async Task RemoveFromCacheAsync(string key)
        {
            await _cache.RemoveAsync(key);
        }
    }
}
