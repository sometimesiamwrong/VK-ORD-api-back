using System.Text.Json.Serialization;

namespace VkOrdApi.Dict;

/// <summary>
/// Ответ с переводами ошибок ЕРИР (GET/POST /v1/dict/erir_message).
/// </summary>
public sealed class VkOrdErirMessageListResponse
{
    /// <summary>
    /// Список переводов (массив объектов message + name).
    /// </summary>
    [JsonPropertyName("items")]
    public List<VkOrdErirMessageItem> Items { get; set; } = new();
}
