using VkOrdApi.Contract;

namespace WebApp.Models.Responses
{
    /// <summary>
    /// Ответ при получении информации о контракте
    /// </summary>
    public class ContractResponse
    {
        /// <summary>
        /// Данные контракта
        /// </summary>
        public VkOrdContract Data { get; set; } = new();

        /// <summary>
        /// Внешний ID контракта
        /// </summary>
        public string ExternalId { get; set; } = string.Empty;
    }

    public class GetContractResponseDto
    {
        public List<VkOrdContract> Data { get; set; } = new();
        public int TotalCount => Data?.Count ?? 0;
        public int TotalItemsCount { get; set; } // Общее количество элементов в VK ORD
        public int Limit { get; set; } // Лимит элементов за запрос
    }
}
