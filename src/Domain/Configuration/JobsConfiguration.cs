using Domain.Entities.Enums;

namespace Jobs.Configuration;

/// <summary>
/// Конфигурация для фоновых задач
/// </summary>
public class JobsConfiguration
{
    public const string SectionName = "Jobs";

    /// <summary>
    /// Интервал синхронизации ERIR статусов в минутах
    /// </summary>
    public int ErirSyncIntervalMinutes { get; set; } = 1;

    /// <summary>
    /// Включенные типы сущностей для синхронизации
    /// </summary>
    public List<string> EnabledEntityTypes { get; set; } = new()
    {
        "Counterparty",
        "Contract",
        "Creative",
        "Invoice",
        "Statistic"
    };

    /// <summary>
    /// Размер батча для запросов к VK ORD API
    /// </summary>
    public int BatchSize { get; set; } = 60000;

    /// <summary>
    /// Максимальное количество одновременных запросов при загрузке сущностей
    /// </summary>
    public int MaxConcurrency { get; set; } = 10;

    /// <summary>
    /// Продолжать работу при ошибках
    /// </summary>
    public bool ContinueOnError { get; set; } = true;

    /// <summary>
    /// Получить список включенных типов сущностей как enum
    /// </summary>
    public List<EntityType> GetEnabledEntityTypes()
    {
        return EnabledEntityTypes
            .Select(x => Enum.Parse<EntityType>(x, ignoreCase: true))
            .Distinct()
            .ToList();
    }
}
