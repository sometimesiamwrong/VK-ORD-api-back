using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.VkOrdCache;

/// <summary>
/// Кэш контрагентов VK ORD API
/// </summary>
[Table("VkOrdCounterpartyCache")]
public class VkOrdCounterpartyCache : VkOrdCacheBase
{
    /// <summary>
    /// ИНН контрагента
    /// </summary>
    [MaxLength(12)]
    public string? Inn { get; set; }

    /// <summary>
    /// Название контрагента
    /// </summary>
    [MaxLength(500)]
    public string? Name { get; set; }

    /// <summary>
    /// URL сайта контрагента
    /// </summary>
    [MaxLength(500)]
    public string? RsUrl { get; set; }

    /// <summary>
    /// Роли контрагента (JSON)
    /// </summary>
    [Column(TypeName = "jsonb")]
    public string? Roles { get; set; }

    /// <summary>
    /// Юридические детали (JSON)
    /// </summary>
    [Column(TypeName = "jsonb")]
    public string? JuridicalDetails { get; set; }

    /// <summary>
    /// Дата последнего обновления в VK ORD
    /// </summary>
    public DateTimeOffset? LastUpdatedInVkOrd { get; set; }

    /// <summary>
    /// Статус синхронизации
    /// </summary>
    public VkOrdSyncStatus SyncStatus { get; set; } = VkOrdSyncStatus.Synced;

    /// <summary>
    /// Договоры, где контрагент является клиентом
    /// </summary>
    public virtual ICollection<VkOrdContractParty> ClientContracts { get; set; } = new List<VkOrdContractParty>();

    /// <summary>
    /// Договоры, где контрагент является подрядчиком
    /// </summary>
    public virtual ICollection<VkOrdContractParty> ContractorContracts { get; set; } = new List<VkOrdContractParty>();

    /// <summary>
    /// Связи с другими контрагентами
    /// </summary>
    public virtual ICollection<VkOrdCounterpartyRelation> Relations { get; set; } = new List<VkOrdCounterpartyRelation>();
}

/// <summary>
/// Статус синхронизации с VK ORD
/// </summary>
public enum VkOrdSyncStatus
{
    /// <summary>
    /// Синхронизирован
    /// </summary>
    Synced = 0,

    /// <summary>
    /// Требует обновления
    /// </summary>
    NeedsUpdate = 1,

    /// <summary>
    /// Ошибка синхронизации
    /// </summary>
    SyncError = 2,

    /// <summary>
    /// Удален в VK ORD
    /// </summary>
    DeletedInVkOrd = 3
}
