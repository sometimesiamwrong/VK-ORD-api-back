namespace WebApp.Models.Creatives;

/// <summary>
/// DTO креатива
/// </summary>
public class CreativeDto
{
    /// <summary>
    /// Внешний идентификатор креатива
    /// </summary>
    public string ExternalId { get; set; } = string.Empty;

    /// <summary>
    /// Название креатива
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Бренд
    /// </summary>
    public string? Brand { get; set; }

    /// <summary>
    /// Категория
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// Описание
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Статус синхронизации
    /// </summary>
    public string SyncStatus { get; set; } = "Synced";

    /// <summary>
    /// Дата последнего обновления
    /// </summary>
    public DateTimeOffset LastUpdated { get; set; }
}

