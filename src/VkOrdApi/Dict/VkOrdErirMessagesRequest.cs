using System.Text.Json.Serialization;

namespace VkOrdApi.Dict;

/// <summary>
/// Запрос на получение переводов нескольких ошибок ЕРИР (POST /v1/dict/erir_message).
/// </summary>
public sealed class VkOrdErirMessagesRequest
{
    /// <summary>
    /// Язык перевода (ru/en/cn, default ru, с fallback cn->en->ru).
    /// </summary>
    [JsonPropertyName("lang")]
    public VkOrdDictLang Lang { get; set; } = VkOrdDictLang.Ru;

    /// <summary>
    /// Список сообщений об ошибках для перевода (required).
    /// Пример: ["At least one text is required", "At least one media url is required"]
    /// </summary>
    [JsonPropertyName("messages")]
    public List<string> Messages { get; set; } = new();
}
