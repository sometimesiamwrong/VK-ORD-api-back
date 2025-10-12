using System.Text.Json.Serialization;

namespace Domain.VkOrdApi.Statistics;

/// <summary>
/// Запрос для создания статистики (POST /v2/statistics). Прямой массив items (minItems 1).
/// </summary>
public sealed class VkOrdApiCreateStatisticsRequest
{
    /// <summary>
    /// Массив элементов статистики (nullable false, minItems 1)
    /// </summary>
    [JsonPropertyName("items")]
    public List<VkOrdApiStatisticsItem> Items { get; set; } = new List<VkOrdApiStatisticsItem>();
}
