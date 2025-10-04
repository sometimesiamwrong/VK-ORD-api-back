using System.Text.Json.Serialization;

namespace VkOrdApi.Creative;

public sealed class VkOrdCreativeResponse
{
    /// <summary>
    /// Дата и время создания креатива
    /// </summary>
    [JsonPropertyName("create_date")]
    public string CreateDate { get; set; } = string.Empty;

    /// <summary>
    /// Название креатива
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Описание креатива
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Тип креатива
    /// </summary>
    [JsonPropertyName("type")]
    public VkOrdCreativeType Type { get; set; }

    /// <summary>
    /// URL файла креатива
    /// </summary>
    [JsonPropertyName("file_url")]
    public string? FileUrl { get; set; }

    /// <summary>
    /// Внешний ID связанного договора
    /// </summary>
    [JsonPropertyName("contract_external_id")]
    public string ContractExternalId { get; set; } = string.Empty;

    /// <summary>
    /// Внешний ID связанной платформы
    /// </summary>
    [JsonPropertyName("pad_external_id")]
    public string PadExternalId { get; set; } = string.Empty;

    /// <summary>
    /// Статус креатива
    /// </summary>
    [JsonPropertyName("status")]
    public VkOrdCreativeStatus Status { get; set; }

    /// <summary>
    /// Дата одобрения или отклонения
    /// </summary>
    [JsonPropertyName("approval_date")]
    public string? ApprovalDate { get; set; }

    /// <summary>
    /// Причины отклонения (если status = rejected)
    /// </summary>
    [JsonPropertyName("rejection_reasons")]
    public List<string> RejectionReasons { get; set; } = new();

    /// <summary>
    /// Настройки таргетинга
    /// </summary>
    [JsonPropertyName("targeting")]
    public Dictionary<string, object>? Targeting { get; set; }
}
