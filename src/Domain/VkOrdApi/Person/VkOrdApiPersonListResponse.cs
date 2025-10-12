using System.Text.Json.Serialization;

namespace Domain.VkOrdApi.Person
{
    /// <summary>
    /// Ответ VK ORD API при получении списка контрагентов /v1/person
    /// </summary>
    public sealed class VkOrdApiPersonListResponse
    {
        /// <summary>
        /// Внешние ID контрагентов
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
}
