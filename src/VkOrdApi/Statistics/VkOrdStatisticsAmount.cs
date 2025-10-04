using System.Text.Json.Serialization;

namespace VkOrdApi.Statistics;

/// <summary>
/// Сумма статистики (object in StatisticsV2Item)
/// </summary>
public sealed class VkOrdStatisticsAmount
{
    /// <summary>
    /// Сумма без НДС (decimal, precision up to 5)
    /// </summary>
    [JsonPropertyName("excluding_vat")]
    public decimal ExcludingVat { get; set; }

    /// <summary>
    /// Ставка НДС (% decimal)
    /// </summary>
    [JsonPropertyName("vat_rate")]
    public decimal VatRate { get; set; }

    /// <summary>
    /// Сумма НДС (decimal, Scale5)
    /// </summary>
    [JsonPropertyName("vat")]
    public decimal Vat { get; set; }

    /// <summary>
    /// Сумма с НДС (decimal, Scale5)
    /// </summary>
    [JsonPropertyName("including_vat")]
    public decimal IncludingVat { get; set; }
}
