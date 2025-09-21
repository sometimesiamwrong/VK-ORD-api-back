using VkOrdApiWrapper.Models.VkOrd;

namespace VkOrdApiWrapper.Models.Responses
{
    /// <summary>
    /// Ответ при создании контракта
    /// </summary>
    public class CreateContractResponse
    {
        /// <summary>
        /// Внешний ID контракта
        /// </summary>
        public string ExternalId { get; set; }

        /// <summary>
        /// Успешно ли создан контракт
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Сообщение об ошибке
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Дата создания
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// Ответ при получении информации о контракте
    /// </summary>
    public class ContractResponse : ApiResponse<VkOrdContract>
    {
        /// <summary>
        /// Внешний ID контракта
        /// </summary>
        public string ExternalId { get; set; }

        /// <summary>
        /// Создать успешный ответ
        /// </summary>
        public static ContractResponse FromVkOrdResponse(VkOrdResponse<VkOrdContract> vkOrdResponse, string externalId)
        {
            if (vkOrdResponse.IsSuccess)
            {
                return new ContractResponse
                {
                    Success = true,
                    Message = "Contract found",
                    Data = vkOrdResponse.Data,
                    ExternalId = externalId
                };
            }
            else
            {
                return new ContractResponse
                {
                    Success = false,
                    Message = vkOrdResponse.Error ?? "Contract not found",
                    ExternalId = externalId
                };
            }
        }
    }
}
