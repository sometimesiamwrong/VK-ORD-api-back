using System.Text.Json.Serialization;

namespace Domain.VkOrdApi.Statistics;

/// <summary>
/// Запрос для удаления статистики (POST /v1/statistics/delete)
/// </summary>
public sealed class VkOrdApiDeleteStatisticsRequest
{
    /// <summary>
    /// Список внешних ID статистик для удаления (array, min 1, unique)
    /// </summary>
    [JsonPropertyName("external_ids")]
    public List<string> ExternalIds { get; set; } = new();
}
