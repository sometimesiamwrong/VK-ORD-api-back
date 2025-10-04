using System.Text.Json.Serialization;

namespace VkOrdApi.Contract;

/// <summary>
/// Ответ VK ORD API при получении списка договоров /v1/contract
/// </summary>
public sealed class VkOrdContractListResponse
{
    /// <summary>
    /// Внешние ID договоров
    /// </summary>
    [JsonPropertyName("external_ids")]
    public List<string> ExternalIds { get; set; } = new();

    /// <summary>
    /// Общее количество элементов в VK ORD
    /// </summary>
    [JsonPropertyName("total_items_count")]
    public int TotalItemsCount { get; set; }

    /// <summary>
    /// Лимит элементов за запрос
    /// </summary>
    [JsonPropertyName("limit")]
    public int Limit { get; set; }
}
