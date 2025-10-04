using System.Text.Json.Serialization;

namespace VkOrdApi.Pad;

public sealed class VkOrdPadResponse
{
    /// <summary>
    /// Дата и время создания платформы (формат date-time).
    /// </summary>
    [JsonPropertyName("create_date")]
    public string CreateDate { get; set; } = string.Empty;

    /// <summary>
    /// Название платформы (maxLength: 255).
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// URL платформы (формат URL).
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Тип платформы (enum: web, mobile_app, social_network, other).
    /// </summary>
    [JsonPropertyName("type")]
    public VkOrdPadType Type { get; set; }

    /// <summary>
    /// Описание платформы (maxLength: 1000).
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }
}
