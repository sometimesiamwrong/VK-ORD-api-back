using System.Text.Json.Serialization;

namespace Domain.VkOrdApi.Invoice;

public sealed class VkOrdApiSendInvoiceToErirRequest
{
    /// <summary>
    /// Комментарий к отправке (опционально)
    /// </summary>
    [JsonPropertyName("comment")]
    public string? Comment { get; set; }
}
