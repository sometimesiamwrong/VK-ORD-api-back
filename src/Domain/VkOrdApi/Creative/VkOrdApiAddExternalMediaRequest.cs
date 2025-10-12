using System.Text.Json.Serialization;

namespace Domain.VkOrdApi.Creative;

public sealed class VkOrdApiAddExternalMediaRequest
{
    /// <summary>
    /// Список внешних медиа-ресурсов (URLs)
    /// </summary>
    [JsonPropertyName("external_media")]
    public List<string> ExternalMedia { get; set; } = new();
}
