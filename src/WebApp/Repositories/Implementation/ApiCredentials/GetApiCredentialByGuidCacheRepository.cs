using System.Text.Json;
using Domain.Entities;
using Microsoft.Extensions.Caching.Distributed;
using WebApp.Repositories.Interfaces.ApiCredentials;

namespace WebApp.Repositories.Implementation.ApiCredentials;

public class GetApiCredentialByGuidCacheRepository : IGetApiCredentialByGuidRepository
{
    private readonly IGetApiCredentialByGuidRepository _getApiCredentialByGuidRepository;
    private readonly IDistributedCache _cacheRepository;

    public GetApiCredentialByGuidCacheRepository(IGetApiCredentialByGuidRepository getApiCredentialByGuidRepository, IDistributedCache cacheRepository)
    {
        _getApiCredentialByGuidRepository = getApiCredentialByGuidRepository;
        _cacheRepository = cacheRepository;
    }

    public async Task<ApiCredential?> GetByGuidAsync(Guid guid)
    {
        var cached = await GetCachedApiCredentialAsync(guid);
        if (cached != null)
        {
            return cached;
        }

        var result = await _getApiCredentialByGuidRepository.GetByGuidAsync(guid);
        if (result != null)
        {
            await SetCachedApiCredentialAsync(guid, result);
        }
        return result;
    }

    // ApiCredential кэш
    public async Task<ApiCredential?> GetCachedApiCredentialAsync(Guid guid)
    {
        var cacheKey = $"api_credential_{guid}";
        var cachedJson = await _cacheRepository.GetStringAsync(cacheKey);
        if (!string.IsNullOrEmpty(cachedJson))
        {
            return JsonSerializer.Deserialize<ApiCredential>(cachedJson);
        }
        return null;
    }

    public async Task SetCachedApiCredentialAsync(Guid guid, ApiCredential apiCredential)
    {
        var cacheKey = $"api_credential_{guid}";
        var json = JsonSerializer.Serialize(apiCredential);
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
        };
        await _cacheRepository.SetStringAsync(cacheKey, json, options);
    }
}