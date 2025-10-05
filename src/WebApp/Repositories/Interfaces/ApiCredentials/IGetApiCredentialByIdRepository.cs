using Domain.Entities;

namespace WebApp.Repositories.Interfaces.ApiCredentials
{
    /// <summary>
    /// Репозиторий для получения ApiCredential по ID
    /// </summary>
    public interface IGetApiCredentialByIdRepository
    {
        /// <summary>
        /// Получить ApiCredential по ID
        /// </summary>
        Task<ApiCredential?> GetById(long id, CancellationToken cancellationToken);

        /// <summary>
        /// Получить ApiCredential по GUID
        /// </summary>
        Task<ApiCredential?> GetByPublicId(Guid publicId, CancellationToken cancellationToken);
    }
}
