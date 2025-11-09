using Domain.Entities;

namespace Jobs.Services.Interfaces;

/// <summary>
/// Сервис синхронизации ERIR статусов для всех логических аккаунтов
/// </summary>
public interface IErirStatusSyncService
{
    /// <summary>
    /// Синхронизировать ERIR статусы для всех логических аккаунтов
    /// </summary>
    /// <param name="cancellationToken">Токен отмены</param>
    Task SyncAllLogicalAccounts(CancellationToken cancellationToken);

    /// <summary>
    /// Синхронизировать ERIR статусы для конкретного логического аккаунта
    /// </summary>
    /// <param name="logicalAccountId">ID логического аккаунта</param>
    /// <param name="credential">API credentials для аккаунта</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task SyncLogicalAccount(long logicalAccountId, ApiCredential credential, CancellationToken cancellationToken);
}
