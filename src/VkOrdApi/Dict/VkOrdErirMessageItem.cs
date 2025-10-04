using System.Text.Json.Serialization;

namespace VkOrdApi.Dict;

/// <summary>
/// Элемент перевода ошибки ЕРИР (оригинальное сообщение + перевод на lang).
/// </summary>
public sealed class VkOrdErirMessageItem
{
    /// <summary>
    /// Сообщение об ошибке от ЕРИР (e.g., 'At least one text is required').
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Перевод на запрошенный язык (e.g., 'Необходим хотя бы один текст для данной формы креатива' для ru).
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}
