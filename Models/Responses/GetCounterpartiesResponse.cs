using VkOrdApiWrapper.Models.VkOrd;

namespace VkOrdApiWrapper.Models.Responses
{
    /// <summary>
    /// Ответ при получении списка контрагентов с полными данными
    /// </summary>
    public class GetCounterpartiesResponse
    {
        public bool Success { get; set; }
        public List<VkOrdPerson> Counterparties { get; set; } = new();
        public string? ErrorMessage { get; set; }
        public int TotalCount => Counterparties?.Count ?? 0;
        public int TotalItemsCount { get; set; } // Общее количество элементов в VK ORD
        public int Limit { get; set; } // Лимит элементов за запрос
    }

    /// <summary>
    /// Ответ при получении контрагента по external_id
    /// </summary>
    public class GetCounterpartyResponse
    {
        public bool Success { get; set; }
        public string ExternalId { get; set; } = string.Empty;
        public VkOrdPerson? Person { get; set; }
        public string? ErrorMessage { get; set; }
    }
}

