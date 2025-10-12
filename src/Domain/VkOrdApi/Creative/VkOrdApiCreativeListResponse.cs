using System.Text.Json.Serialization;

namespace Domain.VkOrdApi.Creative;

/// <summary>
/// Ответ VK ORD API при получении списка креативов /v1/creative
/// </summary>
public sealed class VkOrdApiCreativeListResponse
{
    /// <summary>
    /// Внешние ID креативов
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
