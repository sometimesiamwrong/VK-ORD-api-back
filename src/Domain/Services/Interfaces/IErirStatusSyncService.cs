using Domain.Entities;
using Domain.VkOrdApi.ErirStatus;

namespace Domain.Services.Interfaces;

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
    /// <param name="erirStatusesByAccount">Опциональный словарь со статусами для всех аккаунтов (для оптимизации)</param>
    Task SyncLogicalAccount(
        long logicalAccountId,
        ApiCredential credential,
        CancellationToken cancellationToken,
        Dictionary<long, List<VkOrdApiErirStatusResponse>>? erirStatusesByAccount = null);
}
