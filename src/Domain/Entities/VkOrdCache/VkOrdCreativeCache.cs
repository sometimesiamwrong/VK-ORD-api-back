using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.VkOrdCache;

/// <summary>
/// Кэш креативов VK ORD API
/// </summary>
[Table("VkOrdCreativeCache")]
public class VkOrdCreativeCache : VkOrdCacheBase
{
    /// <summary>
    /// ERID креатива
    /// </summary>
    [MaxLength(255)]
    public string? Erid { get; set; }

    /// <summary>
    /// Внешний идентификатор контрагента
    /// </summary>
    [MaxLength(255)]
    public string? PersonExternalId { get; set; }

    /// <summary>
    /// KKTU коды (JSON)
    /// </summary>
    [Column(TypeName = "jsonb")]
    public string? Kktus { get; set; }

    /// <summary>
    /// Название креатива
    /// </summary>
    [MaxLength(255)]
    public string? Name { get; set; }

    /// <summary>
    /// Бренд
    /// </summary>
    [MaxLength(255)]
    public string? Brand { get; set; }

    /// <summary>
    /// Категория
    /// </summary>
    [MaxLength(255)]
    public string? Category { get; set; }

    /// <summary>
    /// Описание
    /// </summary>
    [MaxLength(2000)]
    public string? Description { get; set; }

    /// <summary>
    /// Тип оплаты
    /// </summary>
    [MaxLength(50)]
    public string? PayType { get; set; }

    /// <summary>
    /// Форма креатива
    /// </summary>
    [MaxLength(50)]
    public string? Form { get; set; }

    /// <summary>
    /// Таргетинг (JSON)
    /// </summary>
    [Column(TypeName = "jsonb")]
    public string? Targeting { get; set; }

    /// <summary>
    /// Целевые URL (JSON)
    /// </summary>
    [Column(TypeName = "jsonb")]
    public string? TargetUrls { get; set; }

    /// <summary>
    /// Тексты (JSON)
    /// </summary>
    [Column(TypeName = "jsonb")]
    public string? Texts { get; set; }

    /// <summary>
    /// Внешние идентификаторы медиа (JSON)
    /// </summary>
    [Column(TypeName = "jsonb")]
    public string? MediaExternalIds { get; set; }

    /// <summary>
    /// Флаги креатива (JSON)
    /// </summary>
    [Column(TypeName = "jsonb")]
    public string? Flags { get; set; }

    /// <summary>
    /// Статус креатива
    /// </summary>
    [MaxLength(50)]
    public string? Status { get; set; }

    /// <summary>
    /// Статус синхронизации
    /// </summary>
    public VkOrdSyncStatus SyncStatus { get; set; } = VkOrdSyncStatus.Synced;

    /// <summary>
    /// Связи с договорами
    /// </summary>
    public virtual ICollection<VkOrdCreativeContract> CreativeContracts { get; set; } = new List<VkOrdCreativeContract>();

    /// <summary>
    /// Связи с медиа
    /// </summary>
    public virtual ICollection<VkOrdCreativeMedia> CreativeMedia { get; set; } = new List<VkOrdCreativeMedia>();

    /// <summary>
    /// Статистика по креативу
    /// </summary>
    public virtual ICollection<VkOrdStatisticsCache> Statistics { get; set; } = new List<VkOrdStatisticsCache>();
}
