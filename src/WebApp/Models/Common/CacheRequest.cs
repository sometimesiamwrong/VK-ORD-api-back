using System.ComponentModel.DataAnnotations;

namespace WebApp.Models.Common;

/// <summary>
/// Базовый класс для запросов с поддержкой кэширования
/// </summary>
public abstract class CacheRequest
{
    /// <summary>
    /// Использовать только кэш (без обращения к API)
    /// </summary>
    public bool CacheOnly { get; set; } = false;

    /// <summary>
    /// Принудительно обновить кэш
    /// </summary>
    public bool ForceRefresh { get; set; } = false;

    /// <summary>
    /// Время жизни кэша в минутах (по умолчанию 60)
    /// </summary>
    [Range(1, 1440)] // от 1 минуты до 24 часов
    public int CacheTtlMinutes { get; set; } = 60;

    /// <summary>
    /// Порог обновления кэша (0.0-1.0, по умолчанию 0.8)
    /// </summary>
    [Range(0.0, 1.0)]
    public double RefreshThreshold { get; set; } = 0.8;

    /// <summary>
    /// Включить подробное логирование операций кэширования
    /// </summary>
    public bool EnableDetailedLogging { get; set; } = false;

    /// <summary>
    /// Теги для группировки кэша
    /// </summary>
    public List<string> CacheTags { get; set; } = new();

    /// <summary>
    /// Приоритет кэширования (выше значение = выше приоритет)
    /// </summary>
    [Range(1, 10)]
    public int CachePriority { get; set; } = 5;
}
