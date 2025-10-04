using System.Text.Json.Serialization;

namespace VkOrdApi.Pad;

/// <summary>
/// Ответ VK ORD API при получении ограниченной информации о платформах /v1/pad/info/restricted
/// </summary>
public sealed class VkOrdPadRestrictedInfoResponse
{
    /// <summary>
    /// Список ограничений для платформ (флаги или правила)
    /// </summary>
    [JsonPropertyName("restrictions")]
    public List<string> Restrictions { get; set; } = new();

    /// <summary>
    /// Описание ограничений
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Актуальность информации (timestamp)
    /// </summary>
    [JsonPropertyName("last_updated")]
    public string? LastUpdated { get; set; }
}
