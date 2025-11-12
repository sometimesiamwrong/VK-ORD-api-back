namespace Domain.Repositories.Interfaces.ApiCredentials
{
    /// <summary>
    /// Репозиторий для удаления ApiCredential
    /// </summary>
    public interface IDeleteApiCredentialRepository
    {
        /// <summary>
        /// Удалить ApiCredential по ID
        /// </summary>
        Task<bool> Delete(Guid id, CancellationToken cancellationToken);
    }
}
