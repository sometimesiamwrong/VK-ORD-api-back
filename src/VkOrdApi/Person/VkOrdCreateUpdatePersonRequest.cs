using System.Text.Json.Serialization;

namespace VkOrdApi.Person;

/// <summary>
/// Запрос на создание/обновление контрагента (Person).
/// </summary>
public sealed class VkOrdCreateUpdatePersonRequest
{
    /// <summary>
    /// Название контрагента.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// URL сайта контрагента (optional).
    /// </summary>
    [JsonPropertyName("rs_url")]
    public string? RsUrl { get; set; }

    /// <summary>
    /// Список ролей контрагента (required array, minItems 1).
    /// Возможные значения: advertiser (рекламодатель), agency (рекламное агентство),
    /// ors (оператор рекламной системы), publisher (издатель, рекламораспространитель).
    /// Пример: ["ors"]
    /// </summary>
    [JsonPropertyName("roles")]
    public List<VkOrdPersonRoles> Roles { get; set; } = new();

    /// <summary>
    /// Юридические детали контрагента.
    /// </summary>
    [JsonPropertyName("juridical_details")]
    public VkOrdPersonJuridicalDetails JuridicalDetails { get; set; } = new();
}