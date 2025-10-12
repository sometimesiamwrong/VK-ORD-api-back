using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.VkOrd;

/// <summary>
/// Связь между креативом и медиа файлами
/// </summary>
public class VkOrdCreativeMedia
{
    /// <summary>
    /// Идентификатор креатива
    /// </summary>
    [Required]
    public long CreativeId { get; set; }

    /// <summary>
    /// Идентификатор медиа файла
    /// </summary>
    [Required]
    public long MediaId { get; set; }

    /// <summary>
    /// Порядок отображения медиа в креативе
    /// </summary>
    [Required]
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
    public virtual VkOrdCreative Creative { get; set; } = null!;

    /// <summary>
    /// Медиа файл
    /// </summary>
    [ForeignKey(nameof(MediaId))]
    public virtual VkOrdMedia Media { get; set; } = null!;
}
