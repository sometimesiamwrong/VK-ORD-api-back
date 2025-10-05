using Domain.Entities;

namespace WebApp.Repositories.Interfaces.ApiCredentials
{
    /// <summary>
    /// Репозиторий для сохранения ApiCredential
    /// </summary>
    public interface ISaveApiCredentialRepository
    {
        /// <summary>
        /// Сохранить ApiCredential (создать или обновить)
        /// </summary>
        Task<ApiCredential?> Save(ApiCredential credential, CancellationToken cancellationToken);
    }
}
