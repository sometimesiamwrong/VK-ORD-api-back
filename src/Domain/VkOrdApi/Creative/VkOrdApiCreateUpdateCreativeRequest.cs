using System.Text.Json.Serialization;
using Domain.Entities.Enums.VkOrd;

namespace Domain.VkOrdApi.Creative;

public sealed class VkOrdApiCreateUpdateCreativeRequest
{
    /// <summary>
    /// Название креатива (обязательное, maxLength: 255)
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Описание креатива (опциональное, maxLength: 1000)
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Тип креатива (обязательное)
    /// </summary>
    [JsonPropertyName("type")]
    public VkOrdApiCreativeForm Type { get; set; }

    /// <summary>
    /// URL файла креатива или base64 content (в зависимости от типа)
    /// </summary>
    [JsonPropertyName("file_url")]
    public string? FileUrl { get; set; }

    /// <summary>
    /// Внешний ID связанного договора
    /// </summary>
    [JsonPropertyName("contract_external_id")]
    public string ContractExternalId { get; set; } = string.Empty;

    /// <summary>
    /// Внешний ID связанной платформы (PAD)
    /// </summary>
    [JsonPropertyName("pad_external_id")]
    public string PadExternalId { get; set; } = string.Empty;

    /// <summary>
    /// Настройки таргетинга (опционально)
    /// </summary>
    [JsonPropertyName("targeting")]
    public Dictionary<string, object>? Targeting { get; set; }
}
