using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.VkOrdCache;

/// <summary>
/// Связь между договором и контрагентом
/// </summary>
[Table("VkOrdContractParty")]
public class VkOrdContractParty
{
    /// <summary>
    /// Идентификатор договора
    /// </summary>
    [Required]
    public long ContractId { get; set; }

    /// <summary>
    /// Идентификатор контрагента
    /// </summary>
    [Required]
    public long CounterpartyId { get; set; }

    /// <summary>
    /// Роль контрагента в договоре
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Role { get; set; } = string.Empty; // "client" или "contractor"

    /// <summary>
    /// Дата создания связи
    /// </summary>
    [Required]
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Договор
    /// </summary>
    [ForeignKey(nameof(ContractId))]
    public virtual VkOrdContractCache Contract { get; set; } = null!;

    /// <summary>
    /// Контрагент
    /// </summary>
    [ForeignKey(nameof(CounterpartyId))]
    public virtual VkOrdCounterpartyCache Counterparty { get; set; } = null!;
}
