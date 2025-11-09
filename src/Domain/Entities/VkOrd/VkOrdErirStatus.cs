using System.ComponentModel.DataAnnotations;
using Domain.Entities.Enums;
using Domain.Entities.Enums.VkOrd;

namespace Domain.Entities.VkOrd;

/// <summary>
/// Таблица для отслеживания ERIR статусов сущностей VK ORD
/// </summary>
public class VkOrdErirStatus : EntityBase
{
    /// <summary>
    /// Идентификатор логического аккаунта
    /// </summary>
    [Required]
    public long LogicalAccountId { get; set; }

    /// <summary>
    /// Внешний идентификатор сущности из VK ORD API
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string ExternalId { get; set; } = null!;

    /// <summary>
    /// Тип сущности (Counterparty, Contract, Creative, Invoice, Statistics)
    /// </summary>
    [Required]
    public EntityType EntityType { get; set; }

    /// <summary>
    /// Статус обработки в ERIR (Processing, Bad, Verified)
    /// </summary>
    [Required]
    public VkOrdApiErirStatus ErirStatus { get; set; }

    /// <summary>
    /// Дата и время последнего обновления статуса пользователем (из VK ORD API)
    /// </summary>
    [Required]
    public DateTimeOffset UpdatedByUserTs { get; set; }

    /// <summary>
    /// Дата и время финализации статуса (когда статус стал Verified или Bad)
    /// </summary>
    public DateTimeOffset? FinalizedTs { get; set; }

    /// <summary>
    /// Список сообщений об ошибках (если статус Bad)
    /// </summary>
    public List<string>? ErrorMessages { get; set; }
}
