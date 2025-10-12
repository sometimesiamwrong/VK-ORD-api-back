using System.Text.Json.Serialization;

namespace Domain.VkOrdApi.Creative;

public sealed class VkOrdApiAddTextRequest
{
    /// <summary>
    /// Список текстов для добавления
    /// </summary>
    [JsonPropertyName("texts")]
    public List<VkOrdTextItem> Texts { get; set; } = new();
}

/// <summary>
/// Элемент текста
/// </summary>
public sealed class VkOrdTextItem
{
    /// <summary>
    /// Текст
    /// </summary>
    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Позиция (опционально)
    /// </summary>
    [JsonPropertyName("position")]
    public string? Position { get; set; }
}
