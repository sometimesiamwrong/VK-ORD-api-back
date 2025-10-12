using System.Text.Json.Serialization;
using Domain.Entities.Enums.VkOrd;

namespace Domain.VkOrdApi.Creative;

public sealed class VkOrdApiCreativeV2Request
{
    /// <summary>
    /// Название креатива (v2)
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Описание (v2)
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Тип креатива (v2)
    /// </summary>
    [JsonPropertyName("type")]
    public VkOrdApiCreativeForm Type { get; set; }

    /// <summary>
    /// URL файла (v2)
    /// </summary>
    [JsonPropertyName("file_url")]
    public string? FileUrl { get; set; }

    /// <summary>
    /// Contract external_id (v2)
    /// </summary>
    [JsonPropertyName("contract_external_id")]
    public string ContractExternalId { get; set; } = string.Empty;

    /// <summary>
    /// Pad external_id (v2)
    /// </summary>
    [JsonPropertyName("pad_external_id")]
    public string PadExternalId { get; set; } = string.Empty;

    /// <summary>
    /// Targeting (v2)
    /// </summary>
    [JsonPropertyName("targeting")]
    public Dictionary<string, object>? Targeting { get; set; }

    /// <summary>
    /// V2-specific field (e.g., format or preview)
    /// </summary>
    [JsonPropertyName("v2_format")]
    public string? V2Format { get; set; }
}
