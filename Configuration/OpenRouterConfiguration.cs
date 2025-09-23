namespace VkOrdApiWrapper.Configuration;

public class OpenRouterConfiguration
{
    public const string SectionName = "OpenRouterSettings";

    public string BaseUrl { get; set; } = "https://openrouter.ai";
    public string ApiKey { get; set; } = string.Empty;
    public int TimeoutSeconds { get; set; } = 30;
    public string Model { get; set; } = "anthropic/claude-3-haiku";
}
