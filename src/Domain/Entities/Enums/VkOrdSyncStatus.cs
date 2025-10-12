namespace Domain.Entities.Enums;

/// <summary>
/// Статус синхронизации с VK ORD
/// </summary>
public enum VkOrdSyncStatus
{
    /// <summary>
    /// Синхронизирован
    /// </summary>
    Synced = 0,

    /// <summary>
    /// Требует обновления
    /// </summary>
    NeedsUpdate = 1,

    /// <summary>
    /// Ошибка синхронизации
    /// </summary>
    SyncError = 2,

    /// <summary>
    /// Удален в VK ORD
    /// </summary>
    DeletedInVkOrd = 3
}