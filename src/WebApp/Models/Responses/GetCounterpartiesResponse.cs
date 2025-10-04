using VkOrdApi.Person;

namespace WebApp.Models.Responses
{
    /// <summary>
    /// Ответ при получении списка контрагентов с полными данными
    /// </summary>
    public class GetCounterpartiesResponse
    {
        public List<VkOrdPersonResponse> Counterparties { get; set; } = new();
        public int TotalCount => Counterparties?.Count ?? 0;
        public int TotalItemsCount { get; set; } // Общее количество элементов в VK ORD
        public int Limit { get; set; } // Лимит элементов за запрос
    }

    /// <summary>
    /// Ответ при получении контрагента по external_id
    /// </summary>
    public class GetCounterpartyResponse
    {
        public string ExternalId { get; set; } = string.Empty;
        public VkOrdPersonResponse? Person { get; set; }
    }
}

