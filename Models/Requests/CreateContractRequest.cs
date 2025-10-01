using System.ComponentModel.DataAnnotations;

namespace VkOrdApiWrapper.Models.Requests
{
    /// <summary> 
    /// Запрос на создание/обновление контракта (от заказчика к исполнителю) VK ОРД
    /// </summary>
    public class CreateContractRequest : AuthorizedRequestBase
    {
        /// <summary>
        /// Внешний ID контракта
        /// </summary>
        [Required]
        public string ExternalId { get; set; }

        /// <summary>
        /// Внешний Id заказчика (Id контрагента в VK ОРД)
        /// </summary>
        [Required]
        public string ClientExternalId { get; set; }

        /// <summary>
        /// Внешний ID Исполнителя (Id контрагента в VK ОРД)
        /// </summary>
        [Required]
        public string ContractorExternalId { get; set; }

        /// <summary>
        /// Сумма оплаты
        /// </summary>
        [Required]
        public int PaySum { get; set; }

        /// <summary>
        /// Дата оплаты
        /// </summary>
        public string? PayDateEnd { get; set; }
    }
}
