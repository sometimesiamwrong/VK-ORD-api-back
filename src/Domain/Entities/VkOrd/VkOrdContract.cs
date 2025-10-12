using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Entities.Enums;
using Domain.VkOrdApi.Contract;

namespace Domain.Entities.VkOrd;

/// <summary>
/// Договор VK ORD API
/// </summary>
public class VkOrdContract : VkOrdBase
{
    /// <summary>
    /// Api контракт.
    /// </summary>
    public VkOrdApiContractResponse Data
    {
        get => GetData<VkOrdApiContractResponse>();
        set => SetData(value);
    }
    
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
    public virtual ICollection<VkOrdContract> AdditionalContracts { get; set; } = new List<VkOrdContract>();
}
