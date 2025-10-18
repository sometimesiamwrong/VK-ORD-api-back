using System.Text.Json.Serialization;

namespace Domain.VkOrdApi.Invoice;

/// <summary>
/// Запрос для удаления договоров из акта (POST /v2/invoice/{external_id}/delete)
/// </summary>
public sealed class VkOrdApiDeleteContractsFromInvoiceRequest
{
    /// <summary>
    /// Список элементов для удаления
    /// </summary>
    [JsonPropertyName("items")]
    public List<VkOrdApiInvoiceItemToDelete> Items { get; set; } = new();
}

/// <summary>
/// Элемент для удаления из акта
/// </summary>
public sealed class VkOrdApiInvoiceItemToDelete
{
    /// <summary>
    /// Внешний идентификатор изначального договора (обязательный)
    /// </summary>
    [JsonPropertyName("contract_external_id")]
    public string ContractExternalId { get; set; } = string.Empty;

    /// <summary>
    /// Список креативов договора для удаления (опционально)
    /// </summary>
    [JsonPropertyName("creatives")]
    public List<VkOrdApiInvoiceCreativeToDelete>? Creatives { get; set; }
}

/// <summary>
/// Креатив для удаления
/// </summary>
public sealed class VkOrdApiInvoiceCreativeToDelete
{
    /// <summary>
    /// Внешний идентификатор креатива (обязательный)
    /// </summary>
    [JsonPropertyName("creative_external_id")]
    public string CreativeExternalId { get; set; } = string.Empty;

    /// <summary>
    /// Список площадок для удаления (опционально)
    /// </summary>
    [JsonPropertyName("platforms")]
    public List<VkOrdApiInvoicePlatformToDelete>? Platforms { get; set; }
}

/// <summary>
/// Площадка для удаления
/// </summary>
public sealed class VkOrdApiInvoicePlatformToDelete
{
    /// <summary>
    /// Внешний идентификатор площадки (обязательный)
    /// </summary>
    [JsonPropertyName("pad_external_id")]
    public string PadExternalId { get; set; } = string.Empty;
}
