using System.Text.Json.Serialization;

namespace VkOrdApi.Creative;

public sealed class VkOrdCreativeV2Response
{
    /// <summary>
    /// Create date (v2)
    /// </summary>
    [JsonPropertyName("create_date")]
    public string CreateDate { get; set; } = string.Empty;

    /// <summary>
    /// Name (v2)
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Description (v2)
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Type (v2)
    /// </summary>
    [JsonPropertyName("type")]
    public VkOrdCreativeType Type { get; set; }

    /// <summary>
    /// File URL (v2)
    /// </summary>
    [JsonPropertyName("file_url")]
    public string? FileUrl { get; set; }

    /// <summary>
    /// Contract ID (v2)
    /// </summary>
    [JsonPropertyName("contract_external_id")]
    public string ContractExternalId { get; set; } = string.Empty;

    /// <summary>
    /// Pad ID (v2)
    /// </summary>
    [JsonPropertyName("pad_external_id")]
    public string PadExternalId { get; set; } = string.Empty;

    /// <summary>
    /// Status (v2)
    /// </summary>
    [JsonPropertyName("status")]
    public VkOrdCreativeStatus Status { get; set; }

    /// <summary>
    /// Approval date (v2)
    /// </summary>
    [JsonPropertyName("approval_date")]
    public string? ApprovalDate { get; set; }

    /// <summary>
    /// Rejection reasons (v2)
    /// </summary>
    [JsonPropertyName("rejection_reasons")]
    public List<string> RejectionReasons { get; set; } = new();

    /// <summary>
    /// Targeting (v2)
    /// </summary>
    [JsonPropertyName("targeting")]
    public Dictionary<string, object>? Targeting { get; set; }

    /// <summary>
    /// ERID (v2, advertising marker)
    /// </summary>
    [JsonPropertyName("erid")]
    public string? Erid { get; set; }
}
