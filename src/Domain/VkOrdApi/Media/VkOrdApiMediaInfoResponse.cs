using System.Text.Json.Serialization;

namespace Domain.VkOrdApi.Media;

public sealed class VkOrdApiMediaInfoResponse
{
    /// <summary>
    /// Название медиафайла
    /// </summary>
    [JsonPropertyName("filename")]
    public string Filename { get; set; } = string.Empty;

    /// <summary>
    /// SHA-256 хеш файла
    /// </summary>
    [JsonPropertyName("sha256")]
    public string Sha256 { get; set; } = string.Empty;

    /// <summary>
    /// Дата создания файла (ISO date-time)
    /// </summary>
    [JsonPropertyName("create_date")]
    public string CreateDate { get; set; } = string.Empty;

    /// <summary>
    /// Размер файла в байтах
    /// </summary>
    [JsonPropertyName("size")]
    public long Size { get; set; }

    /// <summary>
    /// MIME-тип файла (e.g., image/jpeg)
    /// </summary>
    [JsonPropertyName("content_type")]
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    /// Описание медиа (опционально)
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }
}