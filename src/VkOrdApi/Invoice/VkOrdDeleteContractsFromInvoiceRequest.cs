using System.Text.Json.Serialization;

namespace VkOrdApi.Invoice;

public sealed class VkOrdDeleteContractsFromInvoiceRequest
{
    /// <summary>
    /// Список внешних ID договоров для удаления из акта
    /// </summary>
    [JsonPropertyName("contract_external_ids")]
    public List<string> ContractExternalIds { get; set; } = new();
}
