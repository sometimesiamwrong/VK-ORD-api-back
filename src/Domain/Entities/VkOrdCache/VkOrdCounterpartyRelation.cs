using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.VkOrdCache;

/// <summary>
/// Связь между контрагентами
/// </summary>
[Table("VkOrdCounterpartyRelation")]
public class VkOrdCounterpartyRelation
{
    /// <summary>
    /// Идентификатор первого контрагента
    /// </summary>
    [Required]
    public long FromCounterpartyId { get; set; }

    /// <summary>
    /// Идентификатор второго контрагента
    /// </summary>
    [Required]
    public long ToCounterpartyId { get; set; }

    /// <summary>
    /// Тип связи
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string RelationType { get; set; } = string.Empty; // "parent", "subsidiary", "partner", etc.

    /// <summary>
    /// Описание связи
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Дата создания связи
    /// </summary>
    [Required]
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Первый контрагент
    /// </summary>
    [ForeignKey(nameof(FromCounterpartyId))]
    public virtual VkOrdCounterpartyCache FromCounterparty { get; set; } = null!;

    /// <summary>
    /// Второй контрагент
    /// </summary>
    [ForeignKey(nameof(ToCounterpartyId))]
    public virtual VkOrdCounterpartyCache ToCounterparty { get; set; } = null!;
}
