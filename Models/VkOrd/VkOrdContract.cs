using System.Text.Json.Serialization;

namespace VkOrdApiWrapper.Models.VkOrd
{
    /// <summary>
    /// Контракт VK ОРД
    /// </summary>
    public class VkOrdContract
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("client_external_id")]
        public string ClientExternalId { get; set; } = string.Empty;

        [JsonPropertyName("contractor_external_id")]
        public string ContractorExternalId { get; set; } = string.Empty;

        [JsonPropertyName("date")]
        public string Date { get; set; } = string.Empty;

        [JsonPropertyName("date_end")]
        public string DateEnd { get; set; } = string.Empty;

        [JsonPropertyName("serial")]
        public string Serial { get; set; } = string.Empty;

        [JsonPropertyName("action_type")]
        public string ActionType { get; set; } = string.Empty;

        [JsonPropertyName("subject_type")]
        public string SubjectType { get; set; } = string.Empty;

        [JsonPropertyName("flags")]
        public List<string> Flags { get; set; } = new();

        [JsonPropertyName("parent_contract_external_id")]
        public string? ParentContractExternalId { get; set; }

        [JsonPropertyName("amount")]
        public string Amount { get; set; } = string.Empty;
    }
}
