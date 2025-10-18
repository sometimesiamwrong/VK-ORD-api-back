using System.Text.Json.Serialization;

namespace Domain.VkOrdApi.Invoice;

/// <summary>
/// Запрос для добавления договоров в акт (PATCH /v3/invoice/{external_id}/items)
/// </summary>
public sealed class VkOrdApiAddContractsToInvoiceRequest
{
    /// <summary>
    /// Данные по каждому изначальному договору
    /// </summary>
    [JsonPropertyName("items")]
    public List<VkOrdApiInvoiceItemToAdd> Items { get; set; } = new();
}

/// <summary>
/// Элемент для добавления договора в акт
/// </summary>
public sealed class VkOrdApiInvoiceItemToAdd
{
    /// <summary>
    /// Внешний идентификатор изначального договора (обязательный)
    /// </summary>
    [JsonPropertyName("contract_external_id")]
    public string ContractExternalId { get; set; } = string.Empty;

    /// <summary>
    /// Суммы по договору (опционально)
    /// </summary>
    [JsonPropertyName("amount")]
    public VkOrdInvoiceItemAmount? Amount { get; set; }

    /// <summary>
    /// Список креативов в договоре (опционально)
    /// </summary>
    [JsonPropertyName("creatives")]
    public List<VkOrdApiInvoiceItemCreativeToAdd>? Creatives { get; set; }
}

/// <summary>
/// Креатив для добавления в позицию акта
/// </summary>
public sealed class VkOrdApiInvoiceItemCreativeToAdd
{
    /// <summary>
    /// Внешний идентификатор креатива (обязательный)
    /// </summary>
    [JsonPropertyName("creative_external_id")]
    public string CreativeExternalId { get; set; } = string.Empty;

    /// <summary>
    /// Площадки для креатива (опционально)
    /// </summary>
    [JsonPropertyName("platforms")]
    public List<VkOrdApiInvoicePlatform>? Platforms { get; set; }
}

/// <summary>
/// Площадка для креатива в акте
/// </summary>
public sealed class VkOrdApiInvoicePlatform
{
    /// <summary>
    /// Внешний id площадки
    /// </summary>
    [JsonPropertyName("pad_external_id")]
    public string? PadExternalId { get; set; }

    /// <summary>
    /// Количество показов
    /// </summary>
    [JsonPropertyName("shows_count")]
    public string? ShowsCount { get; set; }

    /// <summary>
    /// Количество показов по акту
    /// </summary>
    [JsonPropertyName("invoice_shows_count")]
    public string? InvoiceShowsCount { get; set; }

    /// <summary>
    /// Сумма
    /// </summary>
    [JsonPropertyName("amount")]
    public string? Amount { get; set; }

    /// <summary>
    /// Стоимость за событие
    /// </summary>
    [JsonPropertyName("amount_per_event")]
    public string? AmountPerEvent { get; set; }

    /// <summary>
    /// Дата начала (планируемая)
    /// </summary>
    [JsonPropertyName("date_start_planned")]
    public string? DateStartPlanned { get; set; }

    /// <summary>
    /// Дата окончания (планируемая)
    /// </summary>
    [JsonPropertyName("date_end_planned")]
    public string? DateEndPlanned { get; set; }

    /// <summary>
    /// Дата начала (фактическая)
    /// </summary>
    [JsonPropertyName("date_start_actual")]
    public string? DateStartActual { get; set; }

    /// <summary>
    /// Дата окончания (фактическая)
    /// </summary>
    [JsonPropertyName("date_end_actual")]
    public string? DateEndActual { get; set; }

    /// <summary>
    /// Тип оплаты (например, cpm)
    /// </summary>
    [JsonPropertyName("pay_type")]
    public string? PayType { get; set; }
}
