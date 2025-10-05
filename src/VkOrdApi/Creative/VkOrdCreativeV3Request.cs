using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VkOrdApi.Statistics;

namespace VkOrdApi.Creative;

public sealed class VkOrdCreativeV3Request
{
    /// <summary>
    /// Person external ID (v3)
    /// </summary>
    /// <remarks>
    /// Внешний идентификатор контрагента, для которого создаётся саморекламный креатив. Если вы передали это поле, поле contract_external_id должно отсутствовать.
    /// </remarks>
    [JsonPropertyName("person_external_id")]
    public string? PersonExternalId { get; set; }

    /// <summary>
    /// Contract external IDs (v3)
    /// </summary>
    /// <remarks>
    /// Список внешних идентификаторов изначальных договоров, для которых создается креатив. Если вы передали это поле, идентификатор контрагента должен отсутствовать.
    /// </remarks>
    [JsonPropertyName("contract_external_ids")]
    public List<string> ContractExternalIds { get; set; } = new List<string>();

    /// <summary>
    /// KKTUs (v3)
    /// </summary>
    [JsonPropertyName("kktus")]
    public List<string> Kktus { get; set; } = new List<string>();

    /// <summary>
    /// Name (v3)
    /// </summary>
    [JsonPropertyName("name")]
    [StringLength(255)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Brand (v3)
    /// </summary>
    /// <remarks>
    /// Одно из полей brand, category и description обязательно для заполнения, если в поле kktus единственное значение - "30.15.1" (Прочие товары и услуги).
    /// </remarks>
    [JsonPropertyName("brand")]
    [StringLength(255)]
    public string? Brand { get; set; }

    /// <summary>
    /// Category (v3)
    /// </summary>
    [JsonPropertyName("category")]
    public string? Category { get; set; }

    /// <summary>
    /// Description (v3)
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Pay type (v3)
    /// </summary>
    [JsonPropertyName("pay_type")]
    public VkOrdPayType PayType { get; set; }

    /// <summary>
    /// Form (v3)
    /// </summary>
    [JsonPropertyName("form")]
    public VkOrdCreativeForm Form { get; set; }

    /// <summary>
    /// Targeting (v3)
    /// </summary>
    [JsonPropertyName("targeting")]
    public string? Targeting { get; set; }

    /// <summary>
    /// Target URLs (v3)
    /// </summary>
    [JsonPropertyName("target_urls")]
    public List<string> TargetUrls { get; set; } = new List<string>();

    /// <summary>
    /// Texts (v3)
    /// </summary>
    [JsonPropertyName("texts")]
    public List<string> Texts { get; set; } = new List<string>();

    /// <summary>
    /// Media external IDs (v3)
    /// </summary>
    [JsonPropertyName("media_external_ids")]
    public List<string> MediaExternalIds { get; set; } = new List<string>();

    /// <summary>
    /// Flags (v3)
    /// </summary>
    [JsonPropertyName("flags")]
    public List<VkOrdCreativeFlag> Flags { get; set; } = new List<VkOrdCreativeFlag>();
}
