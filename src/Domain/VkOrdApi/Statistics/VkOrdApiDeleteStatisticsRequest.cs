using System.Text.Json.Serialization;

namespace Domain.VkOrdApi.Statistics;

/// <summary>
/// Запрос для удаления статистики (POST /v1/statistics/delete)
/// </summary>
public sealed class VkOrdApiDeleteStatisticsRequest
{
    /// <summary>
    /// Массив объектов, каждый описывает статистику для удаления
    /// </summary>
    [JsonPropertyName("items")]
    public List<VkOrdApiDeleteStatisticsItem> Items { get; set; } = new();
}

/// <summary>
/// Элемент для удаления статистики
/// </summary>
public sealed class VkOrdApiDeleteStatisticsItem
{
    /// <summary>
    /// Внешний идентификатор креатива
    /// </summary>
    [JsonPropertyName("creative_external_id")]
    public string CreativeExternalId { get; set; } = string.Empty;

    /// <summary>
    /// Внешний идентификатор рекламной площадки
    /// </summary>
    [JsonPropertyName("pad_external_id")]
    public string PadExternalId { get; set; } = string.Empty;

    /// <summary>
    /// Фактическая дата начала рекламной кампании (YYYY-MM-DD)
    /// </summary>
    [JsonPropertyName("date_start_actual")]
    public string DateStartActual { get; set; } = string.Empty;
}
