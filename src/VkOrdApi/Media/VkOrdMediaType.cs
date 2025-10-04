using System.Runtime.Serialization;

namespace VkOrdApi.Media;

public enum VkOrdMediaType
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
