using System.Text.Json.Serialization;

namespace VkOrdApi.Creative;

public sealed class VkOrdAddMediaRequest
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
    public VkOrdCreativeForm? Type { get; set; }
}
