using System.Runtime.Serialization;

namespace Domain.Entities.Enums.VkOrd;

public enum VkOrdApiMediaType
{
    /// <summary>
    /// Изображение
    /// </summary>
    [EnumMember(Value = "image")]
    Image,

    /// <summary>
    /// Видео
    /// </summary>
    [EnumMember(Value = "video")]
    Video,

    /// <summary>
    /// Аудио
    /// </summary>
    [EnumMember(Value = "audio")]
    Audio,

    /// <summary>
    /// Документ
    /// </summary>
    [EnumMember(Value = "document")]
    Document,

    /// <summary>
    /// Иное
    /// </summary>
    [EnumMember(Value = "other")]
    Other
}
