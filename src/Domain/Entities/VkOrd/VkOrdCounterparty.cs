using System.Text.Json.Serialization;
using Domain.Entities.Enums;
using Domain.VkOrdApi.Person;

namespace Domain.Entities.VkOrd;

/// <summary>
/// Контрагент VK ORD API
/// </summary>
public class VkOrdCounterparty : VkOrdBase
{
    /// <summary>
    /// Данные контрагента
    /// </summary>
    public VkOrdApiPersonResponse Data 
    {
        get => GetData<VkOrdApiPersonResponse>();
        set => SetData(value);
    }
    
    public override DateTimeOffset ExpiresAt { get; set; } = DateTimeOffset.UtcNow.AddMinutes(60);

    /// <summary>
    /// Статус синхронизации
    /// </summary>
    public VkOrdSyncStatus SyncStatus { get; set; } = VkOrdSyncStatus.Synced;

    /// <summary>
    /// Связи с договорами
    /// </summary>
    [JsonIgnore]
    public virtual ICollection<VkOrdContractParty> ContractParties { get; set; } = new List<VkOrdContractParty>();
}
