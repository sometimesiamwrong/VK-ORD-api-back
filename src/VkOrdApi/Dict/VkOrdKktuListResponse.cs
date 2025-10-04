using System.Text.Json.Serialization;

namespace VkOrdApi.Dict;

/// <summary>
/// Ответ со списком кодов ККТУ (GET /v1/dict/kktu).
/// Пагинированный, отсортированный по кодам.
/// </summary>
public sealed class VkOrdKktuListResponse
{
    /// <summary>
    /// Общее количество элементов для выдачи по запросу (e.g., 17).
    /// </summary>
    [JsonPropertyName("total_items_count")]
    public int TotalItemsCount { get; set; }

    /// <summary>
    /// Количество элементов в запросе (limit, e.g., 5).
    /// </summary>
    [JsonPropertyName("limit")]
    public int Limit { get; set; }

    /// <summary>
    /// Список элементов ККТУ (отсортированы по code).
    /// </summary>
    [JsonPropertyName("items")]
    public List<VkOrdKktuItem> Items { get; set; } = new();
}
