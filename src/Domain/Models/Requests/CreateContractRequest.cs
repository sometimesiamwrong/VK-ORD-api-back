using System.ComponentModel.DataAnnotations;
using MediatR;

namespace Domain.Models.Requests
{
    /// <summary> 
    /// Запрос на создание/обновление контракта (от заказчика к исполнителю) VK ОРД
    /// </summary>
    public class CreateContractRequest : ICommand<Unit>
    {
        /// <summary>
        /// Внешний ID контракта
        /// </summary>
        [Required]
        public required string ExternalId { get; set; }

        /// <summary>
        /// Внешний Id заказчика (Id контрагента в VK ОРД)
        /// </summary>
        [Required]
        public required string ClientExternalId { get; set; }

        /// <summary>
        /// Внешний ID Исполнителя (Id контрагента в VK ОРД)
        /// </summary>
        [Required]
        public required string ContractorExternalId { get; set; }

        /// <summary>
        /// Сумма оплаты
        /// </summary>
        public int PaySum { get; set; }

        /// <summary>
        /// Дата заключения
        /// </summary>
        public required string Date { get; set; }
        
        /// <summary>
        /// Дата окончания.
        /// </summary>
        public string? DateEnd { get; set; }

        /// <summary>
        /// Серийный номер договора.
        /// </summary>
        public required string Serial { get; set; }
    }
}
