using System.Text.Json.Serialization;

namespace VkOrdApi.Statistics;

/// <summary>
/// Запрос для создания статистики (POST /v2/statistics). Прямой массив items (minItems 1).
/// </summary>
public sealed class VkOrdCreateStatisticsRequest
{
    /// <summary>
    /// Массив элементов статистики (nullable false, minItems 1)
    /// </summary>
    [JsonPropertyName("items")]
    public List<VkOrdStatisticsItem> Items { get; set; } = new List<VkOrdStatisticsItem>();
}
