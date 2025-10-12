using System.Text.Json.Serialization;

namespace Domain.VkOrdApi.Creative;

public sealed class VkOrdApiCreativeV3RequestResponse
{
    /// <summary>
    /// ERID (v3)
    /// </summary>
    [JsonPropertyName("erid")]
    public required string Erid { get; set; }
}
