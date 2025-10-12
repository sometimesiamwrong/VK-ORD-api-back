using System.Text.Json.Serialization;
using Domain.Entities.Enums.VkOrd;
using Domain.VkOrdApi.Statistics;

// for VkOrdPayType

namespace Domain.VkOrdApi.Creative;

public sealed class VkOrdApiCreativeV3Response
{
    /// <summary>
    /// ERID (v3)
    /// </summary>
    [JsonPropertyName("erid")]
    public required string Erid { get; set; }

    /// <summary>
    /// Person external ID (v3)
    /// </summary>
    [JsonPropertyName("person_external_id")]
    public string? PersonExternalId { get; set; }

    /// <summary>
    /// Contract external IDs (v3)
    /// </summary>
    [JsonPropertyName("contract_external_ids")]
    public List<string>? ContractExternalIds { get; set; }

    /// <summary>
    /// KKTUs (v3)
    /// </summary>
    [JsonPropertyName("kktus")]
    public required List<string> Kktus { get; set; }

    /// <summary>
    /// Name (v3)
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    /// <summary>
    /// Brand (v3)
    /// </summary>
    [JsonPropertyName("brand")]
    public string? Brand { get; set; }

    /// <summary>
    /// Category (v3)
    /// </summary>
    [JsonPropertyName("category")]
    public string? Category { get; set; }

    /// <summary>
    /// Description (v3)
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Pay type (v3)
    /// </summary>
    [JsonPropertyName("pay_type")]
    public VkOrdApiPayType ApiPayType { get; set; }

    /// <summary>
    /// Form (v3)
    /// </summary>
    [JsonPropertyName("form")]
    public VkOrdApiCreativeForm Form { get; set; }

    /// <summary>
    /// Targeting (v3)
    /// </summary>
    [JsonPropertyName("targeting")]
    public string? Targeting { get; set; }

    /// <summary>
    /// Target URLs (v3)
    /// </summary>
    [JsonPropertyName("target_urls")]
    public List<string>? TargetUrls { get; set; } = new List<string>();

    /// <summary>
    /// Texts (v3)
    /// </summary>
    [JsonPropertyName("texts")]
    public List<string>? Texts { get; set; } = new List<string>();

    /// <summary>
    /// Media external IDs (v3)
    /// </summary>
    [JsonPropertyName("media_external_ids")]
    public List<string>? MediaExternalIds { get; set; } = new List<string>();

    /// <summary>
    /// Flags (v3)
    /// </summary>
    [JsonPropertyName("flags")]
    public List<VkOrdApiCreativeFlag>? Flags { get; set; } = new List<VkOrdApiCreativeFlag>();
}
