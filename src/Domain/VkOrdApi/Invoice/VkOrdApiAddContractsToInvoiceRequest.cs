using System.Text.Json.Serialization;

namespace Domain.VkOrdApi.Invoice;

public sealed class VkOrdApiAddContractsToInvoiceRequest
{
    /// <summary>
    /// Список внешних ID договоров для добавления в акт
    /// </summary>
    [JsonPropertyName("contract_external_ids")]
    public List<string> ContractExternalIds { get; set; } = new();
}
