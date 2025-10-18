using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Domain.Entities.Enums;
using Domain.VkOrdApi.Invoice;

namespace Domain.Entities.VkOrd;

/// <summary>
/// Акт VK ORD API
/// </summary>
public class VkOrdInvoice : VkOrdBase
{
    /// <summary>
    /// Api акт (полные данные акта из VK ORD API)
    /// </summary>
    public VkOrdApiFullInvoiceResponse Data
    {
        get => GetData<VkOrdApiFullInvoiceResponse>();
        set => SetData(value);
    }
    
    /// <summary>
    /// Статус синхронизации
    /// </summary>
    public VkOrdSyncStatus SyncStatus { get; set; } = VkOrdSyncStatus.Synced;

    /// <summary>
    /// Является ли черновиком
    /// </summary>
    public bool IsDraft { get; set; } = false;

    /// <summary>
    /// Внешний ID основного договора, к которому привязан акт
    /// </summary>
    [MaxLength(255)]
    public string? ContractExternalId { get; set; }

    /// <summary>
    /// Связь с основным договором
    /// </summary>
    [JsonIgnore]
    [ForeignKey(nameof(ContractExternalId))]
    public virtual VkOrdContract? Contract { get; set; }
}
