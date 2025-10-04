namespace WebApp.Configuration
{
    public class DaDataConfiguration
    {
        /// <summary>
        /// Название секции в appsettings.json
        /// </summary>
        public const string SectionName = "DaDataSettings";

        /// <summary>
        /// Базовый URL API DaData
        /// </summary>
        public string BaseUrl { get; set; } = "https://suggestions.dadata.ru/suggestions/api/4_1/rs/";

        /// <summary>
        /// Токен API DaData
        /// </summary>
        public string ApiToken { get; set; } = string.Empty;

        /// <summary>
        /// Таймаут запроса в секундах
        /// </summary>
        public int TimeoutSeconds { get; set; } = 30;
    }
}

