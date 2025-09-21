namespace VkOrdApiWrapper.Configuration
{
    public class DaDataConfiguration
    {
        public const string SectionName = "DaDataSettings";

        public string BaseUrl { get; set; } = "https://suggestions.dadata.ru/suggestions/api/4_1/rs/";
        public string ApiToken { get; set; } = string.Empty;
        public int TimeoutSeconds { get; set; } = 30;
    }
}

