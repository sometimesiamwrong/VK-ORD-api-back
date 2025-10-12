using System.Text.Json.Serialization;
using Domain.Entities.Enums.VkOrd;

namespace Domain.VkOrdApi.Creative;

public sealed class VkOrdApiAddMediaRequest
{
    /// <summary>
    /// Список медиафайлов (URLs or base64)
    /// </summary>
    [JsonPropertyName("media")]
    public List<VkOrdMediaItem> Media { get; set; } = new();
}

/// <summary>
/// Элемент медиа
/// </summary>
public sealed class VkOrdMediaItem
{
    /// <summary>
    /// URL или content медиа
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Тип медиа (image, video, etc.)
    /// </summary>
    [JsonPropertyName("type")]
    public VkOrdApiCreativeForm? Type { get; set; }
}
