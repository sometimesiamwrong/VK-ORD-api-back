using System.Text.Json.Serialization;

namespace VkOrdApi.Invoice;

public sealed class VkOrdInvoiceHeaderRequest
{
    /// <summary>
    /// Номер акта
    /// </summary>
    [JsonPropertyName("number")]
    public string Number { get; set; } = string.Empty;

    /// <summary>
    /// Дата акта
    /// </summary>
    [JsonPropertyName("date")]
    public string Date { get; set; } = string.Empty;

    /// <summary>
    /// Сумма акта
    /// </summary>
    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    /// <summary>
    /// Описание (опционально)
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }
}
