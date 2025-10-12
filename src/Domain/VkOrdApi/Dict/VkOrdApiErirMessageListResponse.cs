using System.Text.Json.Serialization;

namespace Domain.VkOrdApi.Dict;

/// <summary>
/// Ответ с переводами ошибок ЕРИР (GET/POST /v1/dict/erir_message).
/// </summary>
public sealed class VkOrdApiErirMessageListResponse
{
    /// <summary>
    /// Список переводов (массив объектов message + name).
    /// </summary>
    [JsonPropertyName("items")]
    public List<VkOrdApiErirMessageItem> Items { get; set; } = new();
}
