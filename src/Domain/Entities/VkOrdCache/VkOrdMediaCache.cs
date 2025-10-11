using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.VkOrdCache;

/// <summary>
/// Кэш медиа-файлов VK ORD API
/// </summary>
[Table("VkOrdMediaCache")]
public class VkOrdMediaCache : VkOrdCacheBase
{
    /// <summary>
    /// Название файла
    /// </summary>
    [MaxLength(500)]
    public string? Filename { get; set; }

    /// <summary>
    /// SHA-256 хеш файла
    /// </summary>
    [MaxLength(64)]
    public string? Sha256 { get; set; }

    /// <summary>
    /// Дата создания файла
    /// </summary>
    public DateTimeOffset? CreateDate { get; set; }

    /// <summary>
    /// Размер файла в байтах
    /// </summary>
    public long Size { get; set; }

    /// <summary>
    /// MIME-тип файла
    /// </summary>
    [MaxLength(100)]
    public string? ContentType { get; set; }

    /// <summary>
    /// Описание медиа
    /// </summary>
    [MaxLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// Тип медиа
    /// </summary>
    [MaxLength(50)]
    public string? MediaType { get; set; }

    /// <summary>
    /// URL для скачивания
    /// </summary>
    [MaxLength(1000)]
    public string? DownloadUrl { get; set; }

    /// <summary>
    /// Статус загрузки
    /// </summary>
    [MaxLength(50)]
    public string? UploadStatus { get; set; }

    /// <summary>
    /// Статус синхронизации
    /// </summary>
    public VkOrdSyncStatus SyncStatus { get; set; } = VkOrdSyncStatus.Synced;

    /// <summary>
    /// Связи с креативами
    /// </summary>
    public virtual ICollection<VkOrdCreativeMedia> CreativeMedia { get; set; } = new List<VkOrdCreativeMedia>();
}
