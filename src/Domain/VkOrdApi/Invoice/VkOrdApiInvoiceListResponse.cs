using System.Text.Json.Serialization;

namespace Domain.VkOrdApi.Invoice;

/// <summary>
/// Ответ VK ORD API при получении списка актов /v1/invoice
/// </summary>
public sealed class VkOrdApiInvoiceListResponse
{
    /// <summary>
    /// Внешние ID актов
    /// </summary>
    [JsonPropertyName("external_ids")]
    public List<string> ExternalIds { get; set; } = new();

    /// <summary>
    /// Общее количество элементов в VK ORD
    /// </summary>
    [JsonPropertyName("total_items_count")]
    public int TotalItemsCount { get; set; }

    /// <summary>
    /// Лимит элементов за запрос
    /// </summary>
    [JsonPropertyName("limit")]
    public int Limit { get; set; }
}
