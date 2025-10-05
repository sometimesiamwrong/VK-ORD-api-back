using System.Text.Json.Serialization;

namespace VkOrdApi.Creative;

public sealed class VkOrdCreativeV3RequestResponse
{
    /// <summary>
    /// ERID (v3)
    /// </summary>
    [JsonPropertyName("erid")]
    public required string Erid { get; set; }
}
