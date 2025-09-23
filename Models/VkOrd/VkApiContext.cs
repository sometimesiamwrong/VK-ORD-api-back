namespace VkOrdApiWrapper.Models.VkOrd;

/// <summary>
/// Контекст для работы с VK API
/// </summary>
public class VkApiContext
{
    /// <summary>
    /// API ключ VK
    /// </summary>
    public string ApiKey { get; set; }

    /// <summary>
    /// Маршрут API (prod/sandbox)
    /// </summary>
    public string Route { get; set; }

    /// <summary>
    /// Получить базовый URL для API
    /// </summary>
    public string GetBaseUrl()
    {
        return Route?.ToLower() switch
        {
            "prod" or "production" => "https://api.ord.vk.com",
            "sandbox" => "https://api-sandbox.ord.vk.com",
            _ => "https://api-sandbox.ord.vk.com" // по умолчанию sandbox
        };
    }

    /// <summary>
    /// Получить Authorization header
    /// </summary>
    public string GetAuthorizationHeader()
    {
        return $"Bearer {ApiKey}";
    }

    /// <summary>
    /// Проверить валидность контекста
    /// </summary>
    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(ApiKey) && !string.IsNullOrWhiteSpace(Route);
    }
}


