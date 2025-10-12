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
    [JsonPropertyName("total")]
    public int Total { get; set; }

    /// <summary>
    /// Смещение для пагинации.
    /// </summary>
    [JsonPropertyName("offset")]
    public int Offset { get; set; }

    /// <summary>
    /// Лимит на страницу.
    /// </summary>
    [JsonPropertyName("limit")]
    public int Limit { get; set; }
}
