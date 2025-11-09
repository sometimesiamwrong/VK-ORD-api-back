using System.Text.Json.Serialization;

namespace Domain.VkOrdApi.ErirStatus;

/// <summary>
/// Список статусов обработки объектов в ЕРИР (GET/POST /v1/erir_statuses).
/// Пагинированный ответ с элементами статусов (VkOrdErirStatusResponse).
/// </summary>
public sealed class VkOrdApiErirStatusesListResponse
{
    /// <summary>
    /// Список статусов (массив VkOrdErirStatusResponse).
    /// </summary>
    [JsonPropertyName("items")]
    public List<VkOrdApiErirStatusResponse> Items { get; set; } = new();

    /// <summary>
    /// Общее количество элементов.
    /// </summary>
    [JsonPropertyName("total_items_count")]
    public int TotalItemsCount { get; set; }

    /// <summary>
    /// Лимит на страницу.
    /// </summary>
    [JsonPropertyName("limit")]
    public int Limit { get; set; }
    
    /// <summary>
    /// Лимит на страницу.
    /// </summary>
    [JsonPropertyName("limit_per_entity")] 
    public int LimitPerEntity { get; set; }
}
