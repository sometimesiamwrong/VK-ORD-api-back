using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Entities.Enums;
using Domain.VkOrdApi.Media;

namespace Domain.Entities.VkOrd;

/// <summary>
/// Кэш медиа-файлов VK ORD API
/// </summary>
public class VkOrdMedia : VkOrdBase
{
    /// <summary>
    /// Данные медиа-файла
    /// </summary>
    public VkOrdApiMediaInfoResponse Data
    {
        get => GetData<VkOrdApiMediaInfoResponse>();
        set => SetData(value);
    }

    /// <summary>
    /// Связи с медиа
    /// </summary>
    public virtual ICollection<VkOrdCreativeMedia> CreativeMedia { get; set; } = new List<VkOrdCreativeMedia>();

    /// <summary>
    /// Статус синхронизации
    /// </summary>
    public VkOrdSyncStatus SyncStatus { get; set; } = VkOrdSyncStatus.Synced;
}
