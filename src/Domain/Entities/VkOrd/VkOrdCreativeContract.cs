using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.VkOrd;

/// <summary>
/// Связь между креативом и договором
/// </summary>
public class VkOrdCreativeContract
{
    /// <summary>
    /// Идентификатор креатива
    /// </summary>
    [Required]
    public long CreativeId { get; set; }

    /// <summary>
    /// Идентификатор договора
    /// </summary>
    [Required]
    public long ContractId { get; set; }

    /// <summary>
    /// Дата создания связи
    /// </summary>
    [Required]
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Креатив
    /// </summary>
    [ForeignKey(nameof(CreativeId))]
    public virtual VkOrdCreative Creative { get; set; } = null!;

    /// <summary>
    /// Договор
    /// </summary>
    [ForeignKey(nameof(ContractId))]
    public virtual VkOrdContract Contract { get; set; } = null!;
}
