using System.Text.Json.Serialization;

namespace Domain.VkOrdApi.Statistics;

/// <summary>
/// Ответ VK ORD API при получении списка статистик /v2/statistics/list (items array, total_items_count int32, limit int32)
/// </summary>
public sealed class VkOrdApiStatisticsListResponse
{
    /// <summary>
    /// Массив элементов статистики (nullable false)
    /// </summary>
    [JsonPropertyName("items")]
    public List<VkOrdApiStatisticsItem> Items { get; set; } = new List<VkOrdApiStatisticsItem>();

    /// <summary>
    /// Общее количество элементов (int32, example: 13)
    /// </summary>
    [JsonPropertyName("total_items_count")]
    public int TotalItemsCount { get; set; }

    /// <summary>
    /// Лимит элементов за запрос (int32, example: 5)
    /// </summary>
    [JsonPropertyName("limit")]
    public int Limit { get; set; }
}
