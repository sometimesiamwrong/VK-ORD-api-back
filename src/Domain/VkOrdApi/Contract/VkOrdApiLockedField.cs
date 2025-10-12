using System.Text.Json.Serialization;

namespace Domain.VkOrdApi.Contract;

public sealed class VkOrdApiLockedField
{
    /// <summary>
    /// Поле, запрещенное для редактирования.
    /// </summary>
    [JsonPropertyName("field")]
    public string Field { get; set; } = string.Empty;

    /// <summary>
    /// Причины запрета редактирования.
    /// </summary>
    [JsonPropertyName("reasons")]
    public List<string> Reasons { get; set; } = new();
}
