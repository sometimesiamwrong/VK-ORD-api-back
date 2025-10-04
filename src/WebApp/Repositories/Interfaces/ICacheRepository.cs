using Domain.Entities;
using VkOrdApi.Person;
using WebApp.Models.Responses;

namespace WebApp.Repositories.Interfaces
{
    /// <summary>
    /// Репозиторий для работы с кэшем
    /// </summary>
    public interface ICacheRepository
    {
        // DaData кэш
        Task<DaDataPartyShortResponse?> GetCachedPartyByInnAsync(string inn);
        Task SetCachedPartyByInnAsync(string inn, DaDataPartyShortResponse party);

        // Контракт кэш
        Task<string?> GetCachedContractFlagAsync(string externalId);
        Task SetCachedContractFlagAsync(string externalId);

        // Креатив кэш
        Task<string?> GetCachedCreativeAsync(string externalId);
        Task SetCachedCreativeAsync(string externalId, CreateCreativeResponse response);

        // Контрагент кэш
        Task<VkOrdPersonResponse?> GetCachedCounterpartyAsync(Guid userId, string? environment, string externalId);
        Task SetCachedCounterpartyAsync(Guid userId, string? environment, string externalId, VkOrdPersonResponse personResponse);

        // Медиа кэш
        Task<string?> GetCachedMediaAsync(string externalId);
        Task SetCachedMediaAsync(string externalId, UploadMediaResponse response);

        // Удаление из кэша
        Task RemoveFromCacheAsync(string key);
        Task<ApiCredential?> GetCachedApiCredentialAsync(Guid guid);
        Task SetCachedApiCredentialAsync(Guid guid, ApiCredential apiCredential);
    }
}
