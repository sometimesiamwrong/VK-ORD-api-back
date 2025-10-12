using WebApp.Models.Counterparties;

namespace WebApp.Models.Common;

/// <summary>
/// Базовый класс для ответов с информацией о кэшировании
/// </summary>
public abstract class CacheResponse
{
    /// <summary>
    /// Источник данных
    /// </summary>
    public DataSource Source { get; set; }

    /// <summary>
    /// Время получения данных
    /// </summary>
    public DateTimeOffset RetrievedAt { get; set; }

    /// <summary>
    /// Время истечения кэша
    /// </summary>
    public DateTimeOffset? CacheExpiresAt { get; set; }

    /// <summary>
    /// Время создания кэша
    /// </summary>
    public DateTimeOffset? CacheCreatedAt { get; set; }

    /// <summary>
    /// Версия данных в кэше
    /// </summary>
    public int CacheVersion { get; set; }

    /// <summary>
    /// Хеш данных для проверки целостности
    /// </summary>
    public string? DataHash { get; set; }

    /// <summary>
    /// Статистика кэширования
    /// </summary>
    public CacheStatistics? CacheStatistics { get; set; }
}

/// <summary>
/// Статистика кэширования
/// </summary>
public class CacheStatistics
{
    /// <summary>
    /// Время выполнения запроса в миллисекундах
    /// </summary>
    public long ExecutionTimeMs { get; set; }

    /// <summary>
    /// Была ли операция кэширования успешной
    /// </summary>
    public bool CacheHit { get; set; }

    /// <summary>
    /// Размер данных в байтах
    /// </summary>
    public long DataSizeBytes { get; set; }

    /// <summary>
    /// Количество записей в ответе
    /// </summary>
    public int RecordCount { get; set; }

    /// <summary>
    /// Дополнительные метрики
    /// </summary>
    public Dictionary<string, object> Metrics { get; set; } = new();
}
