using System.Text.Json.Serialization;
using Domain.Entities.Enums.VkOrd;

namespace Domain.VkOrdApi.Invoice;

/// <summary>
/// Запрос для создания/обновления заголовка акта (PUT /v3/invoice/{external_id}/header)
/// Без информации о разаллокации
/// </summary>
public sealed class VkOrdApiInvoiceHeaderRequest
{
    /// <summary>
    /// Внешний идентификатор договора, к которому добавляется акт (обязательный)
    /// </summary>
    [JsonPropertyName("contract_external_id")]
    public string ContractExternalId { get; set; } = string.Empty;

    /// <summary>
    /// Внешний идентификатор договора-поручения (опционально)
    /// </summary>
    [JsonPropertyName("order_contract_external_id")]
    public string? OrderContractExternalId { get; set; }

    /// <summary>
    /// Дата выставления акта (обязательный, YYYY-MM-DD)
    /// </summary>
    [JsonPropertyName("date")]
    public string Date { get; set; } = string.Empty;

    /// <summary>
    /// Серийный номер акта (опционально)
    /// </summary>
    [JsonPropertyName("serial")]
    public string? Serial { get; set; }

    /// <summary>
    /// Дата начала периода акта (обязательный, YYYY-MM-DD)
    /// </summary>
    [JsonPropertyName("date_start")]
    public string DateStart { get; set; } = string.Empty;

    /// <summary>
    /// Дата окончания периода акта (обязательный, YYYY-MM-DD)
    /// </summary>
    [JsonPropertyName("date_end")]
    public string DateEnd { get; set; } = string.Empty;

    /// <summary>
    /// Сумма акта (обязательный)
    /// </summary>
    [JsonPropertyName("amount")]
    public VkOrdInvoiceAmount Amount { get; set; } = new();

    /// <summary>
    /// Роль клиента в основном договоре (обязательный)
    /// </summary>
    [JsonPropertyName("client_role")]
    public VkOrdApiClientRole ClientRole { get; set; }

    /// <summary>
    /// Роль подрядчика в основном договоре (обязательный)
    /// </summary>
    [JsonPropertyName("contractor_role")]
    public VkOrdApiClientRole ContractorRole { get; set; }
}
