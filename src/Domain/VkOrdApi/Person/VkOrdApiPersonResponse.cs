using System.Text.Json.Serialization;
using Domain.Entities.Enums.VkOrd;

namespace Domain.VkOrdApi.Person;

/// <summary>
/// Ответ VK ORD API при получении контрагента по external_id /v1/person/{externalId}
/// </summary>
public sealed class VkOrdApiPersonResponse 
{
    /// <summary>
    /// Название контрагента
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// URL сайта контрагента
    /// </summary>
    [JsonPropertyName("rs_url")]
    public string? RsUrl { get; set; }

    /// <summary>
    /// Роли контрагента
    /// </summary>
    [JsonPropertyName("roles")]
    public List<VkOrdApiPersonRoles> Roles { get; set; } = new();

    /// <summary>
    /// Юридические детали контрагента
    /// </summary>
    [JsonPropertyName("juridical_details")]
    public VkOrdApiPersonJuridicalDetails JuridicalDetails { get; set; } = new();
}
