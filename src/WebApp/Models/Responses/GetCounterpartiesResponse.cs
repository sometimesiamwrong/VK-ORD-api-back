using Domain.Entities.VkOrd;
using Domain.VkOrdApi.Person;

namespace WebApp.Models.Responses
{
    public class GetCounterpartiesResponseDto
    {
        public List<VkOrdCounterparty> Data { get; set; } = new();
        public int TotalCount => Data?.Count ?? 0;
        public int TotalItemsCount { get; set; } // Общее количество элементов в VK ORD
        public int Limit { get; set; } // Лимит элементов за запрос
    }

    /// <summary>
    /// Ответ при получении контрагента по external_id
    /// </summary>
    public class GetCounterpartyResponse
    {
        public string ExternalId { get; set; } = string.Empty;
        public VkOrdApiPersonResponse? Data { get; set; }
    }
}

