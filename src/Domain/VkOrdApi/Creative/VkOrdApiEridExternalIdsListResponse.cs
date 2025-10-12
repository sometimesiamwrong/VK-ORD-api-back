using System.Text.Json.Serialization;

namespace Domain.VkOrdApi.Creative;

/// <summary>
/// Ответ VK ORD API при получении списка пар ERID и external_id /v1/creative/list/erid_external_ids
/// </summary>
public sealed class VkOrdApiEridExternalIdsListResponse
{
    /// <summary>
    /// Список пар (ERID, external_id)
    /// </summary>
    [JsonPropertyName("erid_external_ids")]
    public List<VkOrdEridExternalIdPair> EridExternalIds { get; set; } = new();

    /// <summary>
    /// Общее количество элементов
    /// </summary>
    [JsonPropertyName("total_items_count")]
    public int TotalItemsCount { get; set; }

    /// <summary>
    /// Лимит элементов за запрос
    /// </summary>
    [JsonPropertyName("limit")]
    public int Limit { get; set; }
}

/// <summary>
/// Пара ERID и external_id креатива
/// </summary>
public sealed class VkOrdEridExternalIdPair
{
    [JsonPropertyName("erid")]
    public string Erid { get; set; } = string.Empty;

    [JsonPropertyName("external_id")]
    public string ExternalId { get; set; } = string.Empty;
}
