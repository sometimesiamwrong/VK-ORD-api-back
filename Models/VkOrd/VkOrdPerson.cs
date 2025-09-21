
using System.Text.Json.Serialization;

namespace VkOrdApiWrapper.Models.VkOrd
{
    public sealed class VkOrdPerson
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("rs_url")]
        public string? RsUrl { get; set; }

        [JsonPropertyName("roles")]
        public List<string> Roles { get; set; } = new();

        [JsonPropertyName("juridical_details")]
        public VkOrdPersonJuridicalDetails JuridicalDetails { get; set; } = new();
    }

    public sealed class VkOrdPersonJuridicalDetails
	{
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

		[JsonPropertyName("model_scheme")]
		public string ModelScheme { get; set; } = "russia";

        [JsonPropertyName("inn")]
		public string? Inn { get; set; }

        [JsonPropertyName("kpp")]
		public string? Kpp { get; set; }

        [JsonPropertyName("phone")]
		public string? Phone { get; set; }

        [JsonPropertyName("foreign_epayment_method")]
		public string? ForeignEpaymentMethod { get; set; }

        [JsonPropertyName("foreign_registration_number")]
		public string? ForeignRegistrationNumber { get; set; }

        [JsonPropertyName("foreign_inn")]
		public string? ForeignInn { get; set; }

        [JsonPropertyName("foreign_oksm_country_code")]
		public string? ForeignOksmCountryCode { get; set; }
	}
}

