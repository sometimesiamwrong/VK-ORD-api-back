namespace VkOrdApiWrapper.Models.Entities
{
	public class CreativeEntity
	{
		public int Id { get; set; }
		public string ExternalId { get; set; }
		public string Name { get; set; }
		public List<string> ContractExternalIds { get; set; } = new();
		public List<string> KKTYCodes { get; set; } = new();
		public string Format { get; set; }
		public List<string>? ContentUrls { get; set; } = new();
		public string? TargetAudience { get; set; }
		public string? Text { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	}
}

