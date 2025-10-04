using System.Text.Json.Serialization;

namespace VkOrdApi.Statistics;

/// <summary>
/// Элемент статистики (StatisticsV2Item). Требования: for self-ad creative: shows_count=invoice_shows_count=amount=amount_per_event=0. For non-self: if amount_per_event=0 then amount=0.
/// Dates: YYYY-MM-DD, min 1991-01-01, max current UTC; start <= end; same month/year from 2024 for planned/actual.
/// amount_per_event: pattern ^\d{1,11}(\.\d{1,8})?, max 10^10 integer part, 8 decimals for cpm, 5 for others.
/// </summary>
public sealed class VkOrdStatisticsItem
{
    /// <summary>
    /// Внешний идентификатор креатива (обязательный, minLength 1, maxLength 255, example: f13bt882kf-1h785sm9e)
    /// </summary>
    [JsonPropertyName("creative_external_id")]
    public string CreativeExternalId { get; set; } = string.Empty;

    /// <summary>
    /// Внешний идентификатор рекламной площадки (PAD) (обязательный, minLength 1, maxLength 255, example: fe0aqbhhr38-1h4dnaa7a)
    /// </summary>
    [JsonPropertyName("pad_external_id")]
    public string PadExternalId { get; set; } = string.Empty;

    /// <summary>
    /// Количество показов креатива на площадке (обязательный int64, min 0, example: 500)
    /// </summary>
    [JsonPropertyName("shows_count")]
    public long ShowsCount { get; set; }

    /// <summary>
    /// Оплаченное количество показов на площадке (int64, min 0, example: 500)
    /// </summary>
    [JsonPropertyName("invoice_shows_count")]
    public long InvoiceShowsCount { get; set; }

    /// <summary>
    /// Сумма (обязательный object)
    /// </summary>
    [JsonPropertyName("amount")]
    public VkOrdStatisticsAmount Amount { get; set; } = new();

    /// <summary>
    /// Стоимость одного действия (обязательный string decimal, pattern ^\d{1,11}(\.\d{1,8})?, example: 3.98; if 0 then amount=0)
    /// Max integer 10000000000, decimals 8 for cpm, 5 for others.
    /// </summary>
    [JsonPropertyName("amount_per_event")]
    public string AmountPerEvent { get; set; } = string.Empty;

    /// <summary>
    /// Тип оплаты (enum, e.g., cpm, cpc)
    /// </summary>
    [JsonPropertyName("pay_type")]
    public VkOrdPayType PayType { get; set; }

    /// <summary>
    /// Запланированная дата начала кампании (обязательный $date YYYY-MM-DD, min 1991-01-01, max current, <= end_planned, same month/year from 2024)
    /// </summary>
    [JsonPropertyName("date_start_planned")]
    public string DateStartPlanned { get; set; } = string.Empty;

    /// <summary>
    /// Запланированная дата конца кампании ($date, >= start_planned, same constraints)
    /// </summary>
    [JsonPropertyName("date_end_planned")]
    public string DateEndPlanned { get; set; } = string.Empty;

    /// <summary>
    /// Фактическая дата начала кампании (обязательный $date, min 1991-01-01, max current, <= end_actual, same month/year from 2024)
    /// </summary>
    [JsonPropertyName("date_start_actual")]
    public string DateStartActual { get; set; } = string.Empty;

    /// <summary>
    /// Фактическая дата конца кампании ($date, >= start_actual, same constraints)
    /// </summary>
    [JsonPropertyName("date_end_actual")]
    public string DateEndActual { get; set; } = string.Empty;
}
