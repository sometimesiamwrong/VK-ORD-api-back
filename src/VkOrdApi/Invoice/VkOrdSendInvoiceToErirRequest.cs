using System.Text.Json.Serialization;

namespace VkOrdApi.Invoice;

public sealed class VkOrdSendInvoiceToErirRequest
{
    /// <summary>
    /// Комментарий к отправке (опционально)
    /// </summary>
    [JsonPropertyName("comment")]
    public string? Comment { get; set; }
}
