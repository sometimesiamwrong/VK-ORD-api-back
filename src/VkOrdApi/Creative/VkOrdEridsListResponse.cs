using System.Text.Json.Serialization;

namespace VkOrdApi.Creative;

/// <summary>
/// Ответ VK ORD API при получении списка маркеров рекламы /v1/creative/list/erids
/// </summary>
public sealed class VkOrdEridsListResponse
{
    /// <summary>
    /// Список ERID (маркеров рекламы)
    /// </summary>
    [JsonPropertyName("erids")]
    public List<string> Erids { get; set; } = new();

    /// <summary>
    /// Общее количество элементов
    /// </summary>
    [JsonPropertyName("total_items_count")]
    public int TotalItemsCount { get; set; }

    /// <summary>
    /// Лимит элементов за запрос
    /// </summary>
    [JsonPropertyName("limit")]
    public int Limit { get; set; }
}
