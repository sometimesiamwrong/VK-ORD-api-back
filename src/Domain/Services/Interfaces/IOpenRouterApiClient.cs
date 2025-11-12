using System.Text.Json.Serialization;
using Refit;

namespace Domain.Services.Interfaces;

public interface IOpenRouterApiClient
{
    [Post("/api/v1/chat/completions")]
    Task<OpenRouterResponse> GetChatCompletionAsync([Body] OpenRouterRequest request);
}

public class OpenRouterRequest
{
    [JsonPropertyName("model")]
    public string Model { get; set; } = "x-ai/grok-4-fast:free";
    [JsonPropertyName("messages")]
    public List<OpenRouterMessage> Messages { get; set; } = new();
    [JsonPropertyName("temperature")]
    public double Temperature { get; set; } = 0.1;
}

public class OpenRouterMessage
{
    [JsonPropertyName("role")]
    public string Role { get; set; } = "user";
    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;
}

public class OpenRouterResponse
{
    [JsonPropertyName("choices")]
    public List<OpenRouterChoice> Choices { get; set; } = new();
}

public class OpenRouterChoice
{
    [JsonPropertyName("message")]
    public OpenRouterMessage Message { get; set; } = new();
}
