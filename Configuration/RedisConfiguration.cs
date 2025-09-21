namespace VkOrdApiWrapper.Configuration
{
	public class RedisConfiguration
	{
		public const string SectionName = "Redis";
		public string Configuration { get; set; } = "localhost:6379";
		public string InstanceName { get; set; } = "VkOrdApi:";
	}
}

