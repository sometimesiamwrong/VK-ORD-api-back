using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Entities.Enums;
using Domain.VkOrdApi.Creative;

namespace Domain.Entities.VkOrd;

/// <summary>
/// Кэш креативов VK ORD API
/// </summary>
public class VkOrdCreative : VkOrdBase
{
    /// <summary>
    /// Данные креатива
    /// </summary>
    public VkOrdApiCreativeV3Response Data
    {
        get => GetData<VkOrdApiCreativeV3Response>();
        set => SetData(value);
    }

    /// <summary>
    /// Идентификатор договора
    /// </summary>
    [Required]
    public long ContractId { get; set; }

    /// <summary>
    /// Статус синхронизации
    /// </summary>
    public VkOrdSyncStatus SyncStatus { get; set; } = VkOrdSyncStatus.Synced;

    /// <summary>
    /// Связи с медиа
    /// </summary>
    public virtual ICollection<VkOrdCreativeMedia> CreativeMedia { get; set; } = new List<VkOrdCreativeMedia>();

    /// <summary>
    /// Связи с договорами
    /// </summary>
    public virtual ICollection<VkOrdCreativeContract> CreativeContracts { get; set; } = new List<VkOrdCreativeContract>();

    /// <summary>
    /// Статистика по креативу
    /// </summary>
    public virtual ICollection<VkOrdStatistics> Statistics { get; set; } = new List<VkOrdStatistics>();
}
