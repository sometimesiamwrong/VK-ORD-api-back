using Domain.Entities;
using Domain.Entities.Enums;
using Domain.Extensions;
using Domain.Repositories.Interfaces.ApiCredentials;
using Domain.Services.Interfaces;

namespace Domain.Repositories.Implementations.ApiCredentials;

public class GetApiCredentialByGuidCacheRepository : IGetApiCredentialByGuidRepository
{
    private readonly IGetApiCredentialByGuidRepository _getApiCredentialByGuidRepository;
    private readonly ICacheService _cacheService;

    public GetApiCredentialByGuidCacheRepository(
        IGetApiCredentialByGuidRepository getApiCredentialByGuidRepository, 
        ICacheService cacheService)
    {
        _getApiCredentialByGuidRepository = getApiCredentialByGuidRepository;
        _cacheService = cacheService;
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
            await SetCachedApiCredentialAsync(result);
        }
        return result;
    }

    // ApiCredential кэш
    public async Task<ApiCredential?> GetCachedApiCredentialAsync(Guid guid)
    {
        var cacheKey = GetCacheKey(guid);
        return await _cacheService.Get<ApiCredential>(cacheKey, CancellationToken.None);
    }

    public async Task SetCachedApiCredentialAsync(ApiCredential apiCredential)
    {
        var cacheKey = GetCacheKey(apiCredential.PublicId);
        await _cacheService.Save(cacheKey, apiCredential, CancellationToken.None);
    }

    private static string GetCacheKey(Guid guid)
    {
        return $"vkord:{guid}:{EntityType.ApiCredential.GetDescription()}";
    }
}