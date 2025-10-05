using System.Text.Json.Serialization;

namespace VkOrdApi.Media;

/// <summary>
/// Ответ VK ORD API при получении списка медиафайлов /v1/media
/// </summary>
public sealed class VkOrdMediaListResponse
{
    /// <summary>
    /// Внешние ID медиафайлов
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

/// <summary>
/// Ответ VK ORD API при получении списка медиафайлов /v1/media
/// </summary>
public sealed class VkOrdMediaInfoListResponseDto
{
    /// <summary>
    /// Внешние ID медиафайлов
    /// </summary>
    [JsonPropertyName("media")]
    public List<VkOrdMediaInfoResponse> Data { get; set; } = new();

    /// <summary>
    /// Общее количество элементов в VK ORD
    /// </summary>
    public int TotalCount => Data?.Count ?? 0;

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
