using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.VkOrdCache;

/// <summary>
/// Кэш договоров VK ORD API
/// </summary>
[Table("VkOrdContractCache")]
public class VkOrdContractCache : VkOrdCacheBase
{
    /// <summary>
    /// Тип договора
    /// </summary>
    [MaxLength(50)]
    public string? Type { get; set; }

    /// <summary>
    /// Внешний идентификатор клиента
    /// </summary>
    [MaxLength(255)]
    public string? ClientExternalId { get; set; }

    /// <summary>
    /// Внешний идентификатор подрядчика
    /// </summary>
    [MaxLength(255)]
    public string? ContractorExternalId { get; set; }

    /// <summary>
    /// Тип действия (для посреднических договоров)
    /// </summary>
    [MaxLength(50)]
    public string? ActionType { get; set; }

    /// <summary>
    /// Тип предмета договора
    /// </summary>
    [MaxLength(50)]
    public string? SubjectType { get; set; }

    /// <summary>
    /// Дата заключения договора
    /// </summary>
    public DateTime? Date { get; set; }

    /// <summary>
    /// Дата окончания договора
    /// </summary>
    public DateTime? DateEnd { get; set; }

    /// <summary>
    /// Серийный номер договора
    /// </summary>
    [MaxLength(255)]
    public string? Serial { get; set; }

    /// <summary>
    /// Флаги договора (JSON)
    /// </summary>
    [Column(TypeName = "jsonb")]
    public string? Flags { get; set; }

    /// <summary>
    /// Внешний идентификатор родительского договора
    /// </summary>
    [MaxLength(255)]
    public string? ParentContractExternalId { get; set; }

    /// <summary>
    /// Идентификатор родительского договора в кэше
    /// </summary>
    public long? ParentContractId { get; set; }

    /// <summary>
    /// Цена договора
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal? Amount { get; set; }

    /// <summary>
    /// Есть ли дополнительные соглашения
    /// </summary>
    public bool HasAdditionalContracts { get; set; }

    /// <summary>
    /// CID контракта
    /// </summary>
    [MaxLength(255)]
    public string? Cid { get; set; }

    /// <summary>
    /// Заблокированные поля (JSON)
    /// </summary>
    [Column(TypeName = "jsonb")]
    public string? LockedFields { get; set; }

    /// <summary>
    /// Дата создания в VK ORD
    /// </summary>
    public DateTimeOffset? CreateDate { get; set; }

    /// <summary>
    /// Статус синхронизации
    /// </summary>
    public VkOrdSyncStatus SyncStatus { get; set; } = VkOrdSyncStatus.Synced;

    /// <summary>
    /// Связи с контрагентами
    /// </summary>
    public virtual ICollection<VkOrdContractParty> ContractParties { get; set; } = new List<VkOrdContractParty>();

    /// <summary>
    /// Креативы по договору
    /// </summary>
    public virtual ICollection<VkOrdCreativeContract> CreativeContracts { get; set; } = new List<VkOrdCreativeContract>();

    /// <summary>
    /// Дополнительные соглашения
    /// </summary>
    public virtual ICollection<VkOrdContractCache> AdditionalContracts { get; set; } = new List<VkOrdContractCache>();

    /// <summary>
    /// Родительский договор
    /// </summary>
    [ForeignKey(nameof(ParentContractId))]
    public virtual VkOrdContractCache? ParentContract { get; set; }
}
