using System.Text.Json.Serialization;

namespace VkOrdApiWrapper.Models.VkOrd
{
    /// <summary>
    /// Ответ VK ORD API при получении списка контрагентов
    /// </summary>
    public sealed class VkOrdPersonListResponse
    {
        [JsonPropertyName("external_ids")]
        public List<string> ExternalIds { get; set; } = new();

        [JsonPropertyName("total_items_count")]
        public int TotalItemsCount { get; set; }

        [JsonPropertyName("limit")]
        public int Limit { get; set; }
    }
}

