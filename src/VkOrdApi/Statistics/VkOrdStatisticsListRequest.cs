using System.Text.Json.Serialization;

namespace VkOrdApi.Statistics;

/// <summary>
/// Запрос для получения списка статистик (GET /v2/statistics/list)
/// </summary>
public sealed class VkOrdStatisticsListRequest
{
    /// <summary>
    /// Внешний ID креатива для фильтрации
    /// Optional.
    /// </summary>
    [JsonPropertyName("creative_external_id")]
    public string? CreativeExternalId { get; set; }

    /// <summary>
    /// Внешний ID рекламной площадки для фильтрации
    /// Optional.
    /// </summary>
    [JsonPropertyName("pad_external_id")]
    public string? PadExternalId { get; set; }

    /// <summary>
    /// Дата начала фактического показа (от)
    /// Optional.
    /// Format: YYYY-MM-DD
    /// </summary>
    [JsonPropertyName("date_start_actual_from")]
    public string? DateStartActualFrom { get; set; }

    /// <summary>
    /// Дата начала фактического показа (до)
    /// Optional.
    /// Format: YYYY-MM-DD
    /// </summary>
    [JsonPropertyName("date_start_actual_to")]
    public string? DateStartActualTo { get; set; }

    /// <summary>
    /// Количество элементов выдачи, которые необходимо пропустить в запросе
    /// Optional.
    /// Значение по умолчанию: 0
    /// </summary>
    [JsonPropertyName("offset")]
    public int? Offset { get; set; }

    /// <summary>
    /// Количество элементов, которые необходимо получить за один запрос
    /// Optional.
    /// </summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }
}
