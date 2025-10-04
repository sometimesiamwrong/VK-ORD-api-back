using System.Text.Json.Serialization;

namespace VkOrdApi.Creative;

public sealed class VkOrdAddExternalMediaRequest
{
    /// <summary>
    /// Список внешних медиа-ресурсов (URLs)
    /// </summary>
    [JsonPropertyName("external_media")]
    public List<string> ExternalMedia { get; set; } = new();
}
