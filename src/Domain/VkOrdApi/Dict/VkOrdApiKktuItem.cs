using System.Text.Json.Serialization;

namespace Domain.VkOrdApi.Dict;

/// <summary>
/// Элемент списка кодов ККТУ (code + name на указанном языке).
/// </summary>
public sealed class VkOrdApiKktuItem
{
    /// <summary>
    /// Код ККТУ (строка, e.g., '1.2.1').
    /// </summary>
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Описание кода ККТУ на языке запроса (e.g., 'Табачные изделия' для ru).
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}
