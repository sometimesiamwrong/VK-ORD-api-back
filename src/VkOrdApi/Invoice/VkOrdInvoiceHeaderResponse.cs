using System.Text.Json.Serialization;

namespace VkOrdApi.Invoice;

public sealed class VkOrdInvoiceHeaderResponse
{
    /// <summary>
    /// Дата создания
    /// </summary>
    [JsonPropertyName("create_date")]
    public string CreateDate { get; set; } = string.Empty;

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
    /// Статус акта
    /// </summary>
    [JsonPropertyName("status")]
    public VkOrdInvoiceStatus Status { get; set; }

    /// <summary>
    /// Описание
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }
}
