using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.VkOrdCache;

/// <summary>
/// Связь между креативом и медиа-файлом
/// </summary>
[Table("VkOrdCreativeMedia")]
public class VkOrdCreativeMedia
{
    /// <summary>
    /// Идентификатор креатива
    /// </summary>
    [Required]
    public long CreativeId { get; set; }

    /// <summary>
    /// Идентификатор медиа-файла
    /// </summary>
    [Required]
    public long MediaId { get; set; }

    /// <summary>
    /// Порядок медиа в креативе
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Дата создания связи
    /// </summary>
    [Required]
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Креатив
    /// </summary>
    [ForeignKey(nameof(CreativeId))]
    public virtual VkOrdCreativeCache Creative { get; set; } = null!;

    /// <summary>
    /// Медиа-файл
    /// </summary>
    [ForeignKey(nameof(MediaId))]
    public virtual VkOrdMediaCache Media { get; set; } = null!;
}
