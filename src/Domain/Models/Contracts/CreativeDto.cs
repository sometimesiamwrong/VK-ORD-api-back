namespace Domain.Models.Contracts;

/// <summary>
/// DTO креатива
/// </summary>
public class CreativeDto
{
    /// <summary>
    /// Идентификатор в кэше
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Внешний идентификатор из VK ORD API
    /// </summary>
    public string ExternalId { get; set; } = string.Empty;

    /// <summary>
    /// ERID креатива
    /// </summary>
    public string? Erid { get; set; }

    /// <summary>
    /// Внешний идентификатор лица
    /// </summary>
    public string? PersonExternalId { get; set; }

    /// <summary>
    /// Название креатива
    /// </summary>
    public string Name { get; set; } = string.Empty;

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
    /// Тип оплаты
    /// </summary>
    public string? PayType { get; set; }

    /// <summary>
    /// Форма креатива
    /// </summary>
    public string? Form { get; set; }

    /// <summary>
    /// Таргетинг
    /// </summary>
    public string? Targeting { get; set; }

    /// <summary>
    /// Статус
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// Дата и время кэширования
    /// </summary>
    public DateTimeOffset CachedAt { get; set; }

    /// <summary>
    /// Дата и время истечения срока действия кэша
    /// </summary>
    public DateTimeOffset ExpiresAt { get; set; }

    /// <summary>
    /// Дата последнего обновления в VK ORD
    /// </summary>
    public DateTimeOffset? LastUpdated { get; set; }

    /// <summary>
    /// Статус синхронизации
    /// </summary>
    public string SyncStatus { get; set; } = string.Empty;
}