namespace WebApp.Configuration
{
	public class RedisConfiguration
	{	
		/// <summary>
		/// Название секции в appsettings.json
		/// </summary>
		public const string SectionName = "Redis";

		/// <summary>
		/// Строка подключения к Redis
		/// </summary>
		public string Configuration { get; set; } = "localhost:6379";

		/// <summary>
		/// Имя экземпляра Redis
		/// </summary>
		public string InstanceName { get; set; } = "VkOrdApi:";
		/// <summary>
		/// Максимальная длительность чтения из кэша в миллисекундах (по умолчанию 200)
		/// </summary>
		public int ReadTimeoutMs { get; set; } = 50;

		/// <summary>
		/// Максимальная длительность записи в кэш в миллисекундах (по умолчанию 400)
		/// </summary>
		public int WriteTimeoutMs { get; set; } = 100;
	}
}

