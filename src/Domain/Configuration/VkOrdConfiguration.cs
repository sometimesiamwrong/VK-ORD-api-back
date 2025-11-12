namespace WebApp.Configuration
{
    /// <summary>
    /// Конфигурация VK ОРД
    /// </summary>
    public class VkOrdConfiguration
    {
        /// <summary>
        /// Название секции в appsettings.json
        /// </summary>
        public const string SectionName = "VkOrd";

        /// <summary>
        /// Токен API VK ОРД
        /// </summary>
        public required string ApiToken { get; set; }

        /// <summary>
        /// Базовый URL API VK ОРД
        /// </summary>
        public required string BaseUrl { get; set; }

        /// <summary>
        /// Максимальное количество одновременных запросов
        /// </summary>
        public int MaxConcurrentRequests { get; set; } = 5;

        /// <summary>
        /// Таймаут запроса в секундах
        /// </summary>
        public int TimeoutSeconds { get; set; } = 30;

        /// <summary>
        /// Количество попыток повторного запроса
        /// </summary>
        public int RetryCount { get; set; } = 3;

        /// <summary>
        /// Задержка между попытками повторного запроса в миллисекундах
        /// </summary>
        public int RetryDelayMilliseconds { get; set; } = 1000;
    }
}
