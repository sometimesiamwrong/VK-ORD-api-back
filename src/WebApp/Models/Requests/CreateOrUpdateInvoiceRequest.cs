using System.ComponentModel.DataAnnotations;
using Domain.Entities.Enums.VkOrd;
using Domain.VkOrdApi.Invoice;

namespace WebApp.Models.Requests;

/// <summary>
/// Запрос на создание/обновление акта VK ОРД
/// </summary>
public class CreateOrUpdateInvoiceRequest
{
    /// <summary>
    /// Внешний идентификатор акта (устанавливается из маршрута)
    /// </summary>
    public string ExternalId { get; set; } = string.Empty;

    /// <summary>
    /// Внешний идентификатор договора, к которому добавляется акт
    /// </summary>
    [Required]
    [MaxLength(255)]
    public required string ContractExternalId { get; set; }

    /// <summary>
    /// Внешний идентификатор договора-поручения (опционально)
    /// </summary>
    [MaxLength(255)]
    public string? OrderContractExternalId { get; set; }

    /// <summary>
    /// Дата выставления акта (YYYY-MM-DD)
    /// </summary>
    [Required]
    public required string Date { get; set; }

    /// <summary>
    /// Серийный номер акта (опционально)
    /// </summary>
    [MaxLength(255)]
    public string? Serial { get; set; }

    /// <summary>
    /// Дата начала периода акта (YYYY-MM-DD)
    /// </summary>
    [Required]
    public required string DateStart { get; set; }

    /// <summary>
    /// Дата окончания периода акта (YYYY-MM-DD)
    /// </summary>
    [Required]
    public required string DateEnd { get; set; }

    /// <summary>
    /// Сумма акта
    /// </summary>
    [Required]
    public required VkOrdInvoiceAmount Amount { get; set; }

    /// <summary>
    /// Роль клиента в основном договоре
    /// </summary>
    [Required]
    public VkOrdApiClientRole ClientRole { get; set; }

    /// <summary>
    /// Роль подрядчика в основном договоре
    /// </summary>
    [Required]
    public VkOrdApiClientRole ContractorRole { get; set; }

    /// <summary>
    /// Список изначальных договоров в разаллокации (опционально)
    /// </summary>
    public List<VkOrdInvoiceV3Item>? Items { get; set; }

    /// <summary>
    /// Статус акта
    /// </summary>
    public VkOrdApiInvoiceStatus Status { get; set; } = VkOrdApiInvoiceStatus.Draft;
}
