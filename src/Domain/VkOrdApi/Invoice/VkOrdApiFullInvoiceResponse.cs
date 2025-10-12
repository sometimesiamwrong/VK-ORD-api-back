using System.Text.Json.Serialization;
using Domain.Entities.Enums.VkOrd;

namespace Domain.VkOrdApi.Invoice;

/// <summary>
/// Ответ для полного акта (GET /v3/invoice/{external_id}). Зеркалит request + create_date, erir_status (pending/approved/rejected), erir_id (if sent).
/// </summary>
public sealed class VkOrdApiFullInvoiceResponse
{
    /// <summary>
    /// Дата создания акта (ISO date-time)
    /// </summary>
    [JsonPropertyName("create_date")]
    public string CreateDate { get; set; } = string.Empty;

    /// <summary>
    /// Внешний ID договора акта
    /// </summary>
    [JsonPropertyName("contract_external_id")]
    public string ContractExternalId { get; set; } = string.Empty;

    /// <summary>
    /// ID договора-поручения (if set)
    /// </summary>
    [JsonPropertyName("order_contract_external_id")]
    public string? OrderContractExternalId { get; set; }

    /// <summary>
    /// Дата выставления
    /// </summary>
    [JsonPropertyName("date")]
    public string Date { get; set; } = string.Empty;

    /// <summary>
    /// Серийный номер
    /// </summary>
    [JsonPropertyName("serial")]
    public string? Serial { get; set; }

    /// <summary>
    /// Дата начала периода
    /// </summary>
    [JsonPropertyName("date_start")]
    public string DateStart { get; set; } = string.Empty;

    /// <summary>
    /// Дата окончания периода
    /// </summary>
    [JsonPropertyName("date_end")]
    public string DateEnd { get; set; } = string.Empty;

    /// <summary>
    /// Сумма акта (object as in request)
    /// </summary>
    [JsonPropertyName("amount")]
    public VkOrdInvoiceAmount Amount { get; set; } = new();

    /// <summary>
    /// Роль клиента
    /// </summary>
    [JsonPropertyName("client_role")]
    public VkOrdApiClientRole ApiClientRole { get; set; }

    /// <summary>
    /// Роль подрядчика
    /// </summary>
    [JsonPropertyName("contractor_role")]
    public VkOrdApiClientRole ContractorRole { get; set; }

    /// <summary>
    /// Список позиций (as in request)
    /// </summary>
    [JsonPropertyName("items")]
    public List<VkOrdInvoiceV3Item>? Items { get; set; }

    /// <summary>
    /// Статус акта (enum)
    /// </summary>
    [JsonPropertyName("status")]
    public VkOrdApiInvoiceStatus Status { get; set; }

    /// <summary>
    /// Статус в ЕРИР (pending, approved, rejected if sent)
    /// </summary>
    [JsonPropertyName("erir_status")]
    public string? ErirStatus { get; set; }

    /// <summary>
    /// ID в ЕРИР (if sent)
    /// </summary>
    [JsonPropertyName("erir_id")]
    public string? ErirId { get; set; }
}
