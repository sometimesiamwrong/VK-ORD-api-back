using System.Text.Json.Serialization;

namespace VkOrdApi.Creative;

public sealed class VkOrdCreativeV3Response
{
    /// <summary>
    /// Create date (v3)
    /// </summary>
    [JsonPropertyName("create_date")]
    public string CreateDate { get; set; } = string.Empty;

    /// <summary>
    /// Name (v3)
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Description (v3)
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Type (v3)
    /// </summary>
    [JsonPropertyName("type")]
    public VkOrdCreativeType Type { get; set; }

    /// <summary>
    /// File URL (v3)
    /// </summary>
    [JsonPropertyName("file_url")]
    public string? FileUrl { get; set; }

    /// <summary>
    /// Contract ID (v3)
    /// </summary>
    [JsonPropertyName("contract_external_id")]
    public string ContractExternalId { get; set; } = string.Empty;

    /// <summary>
    /// Pad ID (v3)
    /// </summary>
    [JsonPropertyName("pad_external_id")]
    public string PadExternalId { get; set; } = string.Empty;

    /// <summary>
    /// Status (v3)
    /// </summary>
    [JsonPropertyName("status")]
    public VkOrdCreativeStatus Status { get; set; }

    /// <summary>
    /// Approval date (v3)
    /// </summary>
    [JsonPropertyName("approval_date")]
    public string? ApprovalDate { get; set; }

    /// <summary>
    /// Rejection reasons (v3)
    /// </summary>
    [JsonPropertyName("rejection_reasons")]
    public List<string> RejectionReasons { get; set; } = new();

    /// <summary>
    /// Targeting (v3)
    /// </summary>
    [JsonPropertyName("targeting")]
    public Dictionary<string, object>? Targeting { get; set; }

    /// <summary>
    /// ERID (v3)
    /// </summary>
    [JsonPropertyName("erid")]
    public string? Erid { get; set; }

    /// <summary>
    /// V3-specific field (e.g., advanced features)
    /// </summary>
    [JsonPropertyName("v3_advanced")]
    public bool? V3Advanced { get; set; }
}
