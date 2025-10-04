using System.Text.Json.Serialization;

namespace VkOrdApi.Contract;

public sealed class VkOrdLockedField
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
