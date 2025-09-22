namespace VkOrdApiWrapper.Models.Entities
{
	public class ContractEntity
	{
		public int Id { get; set; }
		public string ExternalId { get; set; }
		public string ClientExternalId { get; set; }
		public string ContractorExternalId { get; set; }
		public int PaySum { get; set; }
		public string? PayDateEnd { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	}
}

