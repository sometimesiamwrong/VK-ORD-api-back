using System.Text.Json.Serialization;
using Domain.Entities.Enums.VkOrd;

namespace Domain.VkOrdApi.Invoice;

/// <summary>
/// Запрос для создания полного акта (PUT /v3/invoice/{external_id}). 
/// Структура: root fields + amount object + optional items array.
/// Ограничения: dates in YYYY-MM-DD (min 1991-01-01, max current UTC), amount precision 2, unique contracts/creatives in items, role pairs validation (e.g., client cannot be publisher if no agent_acting_for_publisher).
/// Commission only if main contract type mediation.
/// </summary>
public sealed class VkOrdApiFullInvoiceRequest
{
    /// <summary>
    /// Внешний идентификатор договора, к которому добавляется акт (обязательный, minLength 1, maxLength 255, example: 5t33p36p8p-1h57fjo00)
    /// </summary>
    [JsonPropertyName("contract_external_id")]
    public string ContractExternalId { get; set; } = string.Empty;

    /// <summary>
    /// Внешний идентификатор договора-поручения (nullable, minLength 1, maxLength 255, example: 2k62e25h3q-6t57fn5qe).
    /// Только для main contract type service/mediation. For service: executor = main executor. For mediation: executor = main client.
    /// Only if main has agent_acting_for_publisher flag, and executor matches.
    /// Indicates act contract is by proxy from advertiser.
    /// </summary>
    [JsonPropertyName("order_contract_external_id")]
    public string? OrderContractExternalId { get; set; }

    /// <summary>
    /// Дата выставления акта (обязательный, $date YYYY-MM-DD, min 1991-01-01, max current UTC)
    /// </summary>
    [JsonPropertyName("date")]
    public string Date { get; set; } = string.Empty;

    /// <summary>
    /// Серийный номер акта (nullable, minLength 1, maxLength 255, example: 157). Может отсутствовать.
    /// </summary>
    [JsonPropertyName("serial")]
    public string? Serial { get; set; }

    /// <summary>
    /// Дата начала периода акта (обязательный, $date YYYY-MM-DD, min 1991-01-01, max current UTC, <= date_end)
    /// </summary>
    [JsonPropertyName("date_start")]
    public string DateStart { get; set; } = string.Empty;

    /// <summary>
    /// Дата окончания периода акта (обязательный, $date YYYY-MM-DD, min 1991-01-01, max current UTC, >= date_start)
    /// </summary>
    [JsonPropertyName("date_end")]
    public string DateEnd { get; set; } = string.Empty;

    /// <summary>
    /// Сумма акта (обязательный object)
    /// </summary>
    [JsonPropertyName("amount")]
    public VkOrdInvoiceAmount Amount { get; set; } = new();

    /// <summary>
    /// Роль клиента в main contract (обязательный enum, example: agency). Restrictions on pairs with contractor_role based on contract type/flags (e.g., no publisher if no agent_acting_for_publisher).
    /// Possible: advertiser, agency, ors, publisher, mediator.
    /// </summary>
    [JsonPropertyName("client_role")]
    public VkOrdApiClientRole ApiClientRole { get; set; }

    /// <summary>
    /// Роль подрядчика в main contract (обязательный enum, example: agency). Same enum as client_role, with pair restrictions (e.g., no advertiser for contractor if additional mediation with agent flag).
    /// </summary>
    [JsonPropertyName("contractor_role")]
    public VkOrdApiClientRole ContractorRole { get; set; }

    /// <summary>
    /// Список изначальных договоров в разаллокации (nullable array). Each contract unique, min 1 if present.
    /// API links stats to creatives/contracts/period automatically.
    /// </summary>
    [JsonPropertyName("items")]
    public List<VkOrdInvoiceV3Item>? Items { get; set; }
}

/// <summary>
/// Сумма акта (object)
/// </summary>
public sealed class VkOrdInvoiceAmount
{
    /// <summary>
    /// Информация о сумме за сервисы (обязательный object)
    /// </summary>
    [JsonPropertyName("services")]
    public VkOrdInvoiceServicesAmount Services { get; set; } = new();

    /// <summary>
    /// Информация о вознаграждении посреднику (nullable object, only if main contract mediation).
    /// Cannot if not mediation. Includes serial/date for commission contract.
    /// </summary>
    [JsonPropertyName("commission")]
    public VkOrdInvoiceCommissionAmount? Commission { get; set; }
}

/// <summary>
/// Сумма за сервисы
/// </summary>
public sealed class VkOrdInvoiceServicesAmount
{
    /// <summary>
    /// Сумма без НДС (Scale2 decimal, >0)
    /// </summary>
    [JsonPropertyName("excluding_vat")]
    public decimal ExcludingVat { get; set; }

    /// <summary>
    /// Ставка НДС (%)
    /// </summary>
    [JsonPropertyName("vat_rate")]
    public decimal VatRate { get; set; }

    /// <summary>
    /// Сумма НДС (Scale2 decimal)
    /// </summary>
    [JsonPropertyName("vat")]
    public decimal Vat { get; set; }

    /// <summary>
    /// Сумма с НДС (Scale2 decimal)
    /// </summary>
    [JsonPropertyName("including_vat")]
    public decimal IncludingVat { get; set; }
}

/// <summary>
/// Вознаграждение посреднику (commission)
/// </summary>
public sealed class VkOrdInvoiceCommissionAmount
{
    /// <summary>
    /// Сумма без НДС (Scale2 decimal, >0)
    /// </summary>
    [JsonPropertyName("excluding_vat")]
    public decimal ExcludingVat { get; set; }

    /// <summary>
    /// Ставка НДС (%)
    /// </summary>
    [JsonPropertyName("vat_rate")]
    public decimal VatRate { get; set; }

    /// <summary>
    /// Сумма НДС (Scale2 decimal)
    /// </summary>
    [JsonPropertyName("vat")]
    public decimal Vat { get; set; }

    /// <summary>
    /// Сумма с НДС (Scale2 decimal)
    /// </summary>
    [JsonPropertyName("including_vat")]
    public decimal IncludingVat { get; set; }

    /// <summary>
    /// Серийный номер договора комиссии (minLength 1, max 255, example: 123)
    /// </summary>
    [JsonPropertyName("serial")]
    public string Serial { get; set; } = string.Empty;

    /// <summary>
    /// Дата договора комиссии ($date YYYY-MM-DD)
    /// </summary>
    [JsonPropertyName("date")]
    public string Date { get; set; } = string.Empty;
}

/// <summary>
/// Позиция в items (изначальный договор в разаллокации)
/// </summary>
public sealed class VkOrdInvoiceV3Item
{
    /// <summary>
    /// Внешний ID договора (обязательный, minLength 1, maxLength 255)
    /// </summary>
    [JsonPropertyName("contract_external_id")]
    public string ContractExternalId { get; set; } = string.Empty;

    /// <summary>
    /// Сумма по позиции (object, nullable false)
    /// </summary>
    [JsonPropertyName("amount")]
    public VkOrdInvoiceItemAmount Amount { get; set; } = new();

    /// <summary>
    /// Список креативов позиции (nullable array, unique, API auto-links stats)
    /// </summary>
    [JsonPropertyName("creatives")]
    public List<VkOrdInvoiceItemCreative>? Creatives { get; set; }
}

/// <summary>
/// Сумма позиции (item amount)
/// </summary>
public sealed class VkOrdInvoiceItemAmount
{
    /// <summary>
    /// Сумма без НДС (Scale2 decimal, >0)
    /// </summary>
    [JsonPropertyName("excluding_vat")]
    public decimal ExcludingVat { get; set; }

    /// <summary>
    /// Ставка НДС (%)
    /// </summary>
    [JsonPropertyName("vat_rate")]
    public decimal VatRate { get; set; }

    /// <summary>
    /// Сумма НДС (Scale2 decimal)
    /// </summary>
    [JsonPropertyName("vat")]
    public decimal Vat { get; set; }

    /// <summary>
    /// Сумма с НДС (Scale2 decimal)
    /// </summary>
    [JsonPropertyName("including_vat")]
    public decimal IncludingVat { get; set; }
}

/// <summary>
/// Креатив в позиции items
/// </summary>
public sealed class VkOrdInvoiceItemCreative
{
    /// <summary>
    /// ERID креатива (string, unique in list)
    /// </summary>
    [JsonPropertyName("erid")]
    public string Erid { get; set; } = string.Empty;

    /// <summary>
    /// External ID креатива (if needed, optional)
    /// </summary>
    [JsonPropertyName("external_id")]
    public string? ExternalId { get; set; }
}
