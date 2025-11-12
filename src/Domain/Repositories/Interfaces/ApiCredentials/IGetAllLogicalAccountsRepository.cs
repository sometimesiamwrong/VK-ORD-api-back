using Domain.Entities;

namespace Domain.Repositories.Interfaces.ApiCredentials;

/// <summary>
/// Репозиторий для получения всех логических аккаунтов с credentials
/// </summary>
public interface IGetAllLogicalAccountsRepository
{
    /// <summary>
    /// Получить все уникальные логические аккаунты с их credentials
    /// </summary>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Список пар (LogicalAccountId, ApiCredential)</returns>
    Task<List<(long LogicalAccountId, ApiCredential Credential)>> GetAllWithCredentials(
        CancellationToken cancellationToken);
}
