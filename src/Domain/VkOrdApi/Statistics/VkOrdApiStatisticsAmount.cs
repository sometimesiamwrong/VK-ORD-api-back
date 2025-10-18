using System.Text.Json.Serialization;

namespace Domain.VkOrdApi.Statistics;

/// <summary>
/// Сумма статистики (object in StatisticsV2Item)
/// </summary>
public sealed class VkOrdApiStatisticsAmount
{
    /// <summary>
    /// Неотрицательная сумма без учета налогов. Максимум — 9 999 999 999.
    /// </summary>
    [JsonPropertyName("excluding_vat")]
    public string ExcludingVat { get; set; } = string.Empty;

    /// <summary>
    /// Ставка НДС в процентах. Максимальное значение — 20.
    /// </summary>
    [JsonPropertyName("vat_rate")]
    public string VatRate { get; set; } = string.Empty;

    /// <summary>
    /// Неотрицательная сумма НДС (максимум 20% от excluding_vat + 0.001 коп).
    /// </summary>
    [JsonPropertyName("vat")]
    public string Vat { get; set; } = string.Empty;

    /// <summary>
    /// Сумма с учетом налогов. Должна быть равна excluding_vat + vat.
    /// </summary>
    [JsonPropertyName("including_vat")]
    public string IncludingVat { get; set; } = string.Empty;
}
