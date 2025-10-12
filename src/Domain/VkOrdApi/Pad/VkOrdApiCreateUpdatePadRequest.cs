using System.Text.Json.Serialization;
using Domain.Entities.Enums;

namespace Domain.VkOrdApi.Pad;

public sealed class VkOrdApiCreateUpdatePadRequest
{
    /// <summary>
    /// Название платформы (обязательное поле, maxLength: 255).
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// URL платформы (обязательное поле, формат URL).
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Тип платформы (обязательное поле, enum: web, mobile_app, social_network, other).
    /// </summary>
    [JsonPropertyName("type")]
    public VkOrdApiPadType Type { get; set; }

    /// <summary>
    /// Описание платформы (опциональное, maxLength: 1000).
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }
}
