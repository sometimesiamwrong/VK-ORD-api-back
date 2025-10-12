using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Entities.Enums;

namespace Domain.Entities.VkOrd;

/// <summary>
/// Связь между договором и контрагентом с ролью
/// </summary>
public class VkOrdContractParty
{
    /// <summary>
    /// Идентификатор связи
    /// </summary>
    [Required]
    public long Id { get; set; }

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
    public VkOrdContractRole Role { get; set; }

    /// <summary>
    /// Дата создания связи
    /// </summary>
    [Required]
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Договор
    /// </summary>
    [ForeignKey(nameof(ContractId))]
    public virtual VkOrdContract Contract { get; set; } = null!;

    /// <summary>
    /// Контрагент
    /// </summary>
    [ForeignKey(nameof(CounterpartyId))]
    public virtual VkOrdCounterparty Counterparty { get; set; } = null!;
}
