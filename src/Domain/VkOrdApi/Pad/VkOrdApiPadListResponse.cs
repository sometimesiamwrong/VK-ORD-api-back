using System.Text.Json.Serialization;

namespace Domain.VkOrdApi.Pad;

/// <summary>
/// Ответ VK ORD API при получении списка платформ /v1/pad
/// </summary>
public sealed class VkOrdApiPadListResponse
{
    /// <summary>
    /// Внешние ID платформ
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
