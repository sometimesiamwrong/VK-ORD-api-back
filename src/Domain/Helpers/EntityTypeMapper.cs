using Domain.Entities.Enums;
using Domain.Entities.Enums.VkOrd;

namespace Domain.Helpers;

/// <summary>
/// Helper для маппинга типов сущностей
/// </summary>
public static class EntityTypeMapper
{
    /// <summary>
    /// Конвертирует EntityType в VkOrdApiErirDataType для запросов к ERIR API
    /// </summary>
    /// <param name="entityType">Тип сущности</param>
    /// <returns>Тип данных для ERIR API</returns>
    /// <exception cref="ArgumentException">Если тип сущности не поддерживается</exception>
    public static VkOrdApiErirDataType ToErirDataType(EntityType entityType) => entityType switch
    {
        EntityType.Counterparty => VkOrdApiErirDataType.Person,
        EntityType.Contract => VkOrdApiErirDataType.Contract,
        EntityType.Creative => VkOrdApiErirDataType.Creative,
        EntityType.Invoice => VkOrdApiErirDataType.Invoice,
        EntityType.Statistic => VkOrdApiErirDataType.Statistics,
        _ => throw new ArgumentException($"Unsupported entity type for ERIR sync: {entityType}", nameof(entityType))
    };

    /// <summary>
    /// Получить все поддерживаемые типы сущностей для ERIR синхронизации
    /// </summary>
    /// <returns>Список типов сущностей</returns>
    public static List<EntityType> GetSupportedErirEntityTypes() => new()
    {
        EntityType.Counterparty,
        EntityType.Contract,
        EntityType.Creative,
        EntityType.Invoice,
        EntityType.Statistic
    };

    /// <summary>
    /// Проверяет, поддерживается ли тип сущности для ERIR синхронизации
    /// </summary>
    /// <param name="entityType">Тип сущности</param>
    /// <returns>true если поддерживается</returns>
    public static bool IsErirSyncSupported(EntityType entityType)
    {
        return entityType is EntityType.Counterparty
            or EntityType.Contract
            or EntityType.Creative
            or EntityType.Invoice
            or EntityType.Statistic;
    }
}
