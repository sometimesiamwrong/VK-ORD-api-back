using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Entities.Enums;

namespace Domain.Entities.VkOrd;

/// <summary>
/// Кэш статистики VK ORD API
/// </summary>
public class VkOrdStatistic : VkOrdBase
{
    /// <summary>
    /// Внешний идентификатор креатива
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string CreativeExternalId { get; set; } = string.Empty;

    /// <summary>
    /// Идентификатор креатива в кэше
    /// </summary>
    public long? CreativeId { get; set; }

    /// <summary>
    /// Внешний идентификатор рекламной площадки (PAD)
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string PadExternalId { get; set; } = string.Empty;

    /// <summary>
    /// Количество показов
    /// </summary>
    public long ShowsCount { get; set; }

    /// <summary>
    /// Оплаченное количество показов
    /// </summary>
    public long InvoiceShowsCount { get; set; }

    /// <summary>
    /// Сумма (JSON)
    /// </summary>
    [Column(TypeName = "jsonb")]
    public string? Amount { get; set; }

    /// <summary>
    /// Стоимость одного действия
    /// </summary>
    [Column(TypeName = "decimal(18,8)")]
    public decimal AmountPerEvent { get; set; }

    /// <summary>
    /// Тип оплаты
    /// </summary>
    [MaxLength(50)]
    public string? PayType { get; set; }

    /// <summary>
    /// Запланированная дата начала
    /// </summary>
    public DateTime? DateStartPlanned { get; set; }

    /// <summary>
    /// Запланированная дата окончания
    /// </summary>
    public DateTime? DateEndPlanned { get; set; }

    /// <summary>
    /// Фактическая дата начала
    /// </summary>
    public DateTime? DateStartActual { get; set; }

    /// <summary>
    /// Фактическая дата окончания
    /// </summary>
    public DateTime? DateEndActual { get; set; }

    /// <summary>
    /// Период статистики (YYYY-MM)
    /// </summary>
    [MaxLength(7)]
    public string? Period { get; set; }

    /// <summary>
    /// Тип статистики
    /// </summary>
    [MaxLength(50)]
    public string? StatisticsType { get; set; }

    /// <summary>
    /// Статус синхронизации
    /// </summary>
    public VkOrdSyncStatus SyncStatus { get; set; } = VkOrdSyncStatus.Synced;

    /// <summary>
    /// Креатив
    /// </summary>
    [ForeignKey(nameof(CreativeId))]
    public virtual VkOrdCreative? Creative { get; set; }
}
