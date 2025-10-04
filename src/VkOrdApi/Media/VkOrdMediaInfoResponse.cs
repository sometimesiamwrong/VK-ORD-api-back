using System.Text.Json.Serialization;

namespace VkOrdApi.Media;

public sealed class VkOrdMediaInfoResponse
{
    /// <summary>
    /// Название медиафайла
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Размер файла в байтах
    /// </summary>
    [JsonPropertyName("size")]
    public long Size { get; set; }

    /// <summary>
    /// MIME-тип файла (e.g., image/jpeg)
    /// </summary>
    [JsonPropertyName("mime_type")]
    public string MimeType { get; set; } = string.Empty;

    /// <summary>
    /// Дата загрузки (ISO date-time)
    /// </summary>
    [JsonPropertyName("upload_date")]
    public string UploadDate { get; set; } = string.Empty;

    /// <summary>
    /// Тип медиа (enum)
    /// </summary>
    [JsonPropertyName("type")]
    public VkOrdMediaType Type { get; set; }

    /// <summary>
    /// Описание медиа (опционально)
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// URL для скачивания (опционально)
    /// </summary>
    [JsonPropertyName("file_url")]
    public string? FileUrl { get; set; }
}
