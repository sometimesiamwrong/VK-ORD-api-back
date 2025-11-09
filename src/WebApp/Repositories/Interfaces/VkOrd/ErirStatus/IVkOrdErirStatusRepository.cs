using Domain.Entities.Enums;
using Domain.Entities.VkOrd;

namespace WebApp.Repositories.Interfaces.VkOrd.ErirStatus;

/// <summary>
/// Репозиторий для работы с ERIR статусами сущностей VK ORD
/// </summary>
public interface IVkOrdErirStatusRepository
{
    /// <summary>
    /// Получить статус по external ID
    /// </summary>
    /// <param name="logicalAccountId">ID логического аккаунта</param>
    /// <param name="externalId">External ID сущности</param>
    /// <param name="entityType">Тип сущности</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Статус или null</returns>
    Task<VkOrdErirStatus?> GetByExternalId(
        long logicalAccountId,
        string externalId,
        EntityType entityType,
        CancellationToken cancellationToken);

    /// <summary>
    /// Получить все статусы для логического аккаунта и типа сущности
    /// </summary>
    /// <param name="logicalAccountId">ID логического аккаунта</param>
    /// <param name="entityType">Тип сущности</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Список статусов</returns>
    Task<List<VkOrdErirStatus>> GetAllByLogicalAccount(
        long logicalAccountId,
        EntityType entityType,
        CancellationToken cancellationToken);

    /// <summary>
    /// Создать или обновить статус
    /// </summary>
    /// <param name="status">Статус для сохранения</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task UpsertStatus(VkOrdErirStatus status, CancellationToken cancellationToken);

    /// <summary>
    /// Получить все external ID для логического аккаунта и типа сущности из таблицы статусов
    /// </summary>
    /// <param name="logicalAccountId">ID логического аккаунта</param>
    /// <param name="entityType">Тип сущности</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Список external ID</returns>
    Task<List<string>> GetAllExternalIds(
        long logicalAccountId,
        EntityType entityType,
        CancellationToken cancellationToken);

    /// <summary>
    /// Получить все external ID из основных таблиц VK ORD сущностей для синхронизации
    /// </summary>
    /// <param name="logicalAccountId">ID логического аккаунта</param>
    /// <param name="entityType">Тип сущности</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Список external ID</returns>
    Task<List<string>> GetAllExternalIdsFromVkOrdEntities(
        long logicalAccountId,
        EntityType entityType,
        CancellationToken cancellationToken);
}
