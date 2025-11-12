using Domain.Entities;

namespace Domain.Repositories.Interfaces.ApiCredentials
{
    /// <summary>
    /// Репозиторий для получения ApiCredential по GUID
    /// </summary>
    public interface IGetApiCredentialByGuidRepository
    {
        /// <summary>
        /// Получить ApiCredential по GUID
        /// </summary>
        Task<ApiCredential?> GetByGuidAsync(Guid guid);
    }
}