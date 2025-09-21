using Newtonsoft.Json;

namespace VkOrdApiWrapper.Models.VkOrd
{
	public class VkOrdCounterparty
	{
		[JsonProperty("inn")]
		public string Inn { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("role")]
		public string Role { get; set; } = "advertiser";

		[JsonProperty("type")]
		public string Type { get; set; }
	}
}

