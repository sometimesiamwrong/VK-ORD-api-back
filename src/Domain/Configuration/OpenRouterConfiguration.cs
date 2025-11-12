namespace WebApp.Configuration;

public class OpenRouterConfiguration
{
    /// <summary>
    /// Название секции в appsettings.json
    /// </summary>
    public const string SectionName = "OpenRouterSettings";
    
    /// <summary>
    /// Базовый URL API OpenRouter
    /// </summary>

    public string BaseUrl { get; set; } = "https://openrouter.ai";

    /// <summary>
    /// Токен API OpenRouter
    /// </summary>
    public required string ApiKey { get; set; }

    /// <summary>
    /// Таймаут запроса в секундах
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Модель OpenRouter
    /// </summary>
    public string Model { get; set; } = "anthropic/claude-3-haiku";
}
