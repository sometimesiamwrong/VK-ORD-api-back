using System.Text.Json.Serialization;

namespace VkOrdApi.Invoice;

public sealed class VkOrdAddContractsToInvoiceRequest
{
    /// <summary>
    /// Список внешних ID договоров для добавления в акт
    /// </summary>
    [JsonPropertyName("contract_external_ids")]
    public List<string> ContractExternalIds { get; set; } = new();
}
