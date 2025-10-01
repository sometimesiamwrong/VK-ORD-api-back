using System.Text.Json.Serialization;

namespace VkOrdApiWrapper.Models.VkOrd
{
    /// <summary>
    /// Медиа файл VK ОРД
    /// </summary>
    public class VkOrdMedia
    {
        /// <summary>
        /// Внешний ID медиа файла
        /// </summary>
        [JsonPropertyName("external_id")]
        public string ExternalId { get; set; }

        /// <summary>
        /// URL медиа файла
        /// </summary>
        [JsonPropertyName("url")]
        public string Url { get; set; }

        /// <summary>
        /// Тип медиа файла (image, video, etc.)
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// MIME тип файла
        /// </summary>
        [JsonPropertyName("mime_type")]
        public string MimeType { get; set; }

        /// <summary>
        /// Размер файла в байтах
        /// </summary>
        [JsonPropertyName("size")]
        public long Size { get; set; }

        /// <summary>
        /// Ширина (для изображений)
        /// </summary>
        [JsonPropertyName("width")]
        public int? Width { get; set; }

        /// <summary>
        /// Высота (для изображений)
        /// </summary>
        [JsonPropertyName("height")]
        public int? Height { get; set; }

        /// <summary>
        /// Длительность (для видео)
        /// </summary>
        [JsonPropertyName("duration")]
        public int? Duration { get; set; }

        /// <summary>
        /// ERID токен
        /// </summary>
        [JsonPropertyName("erid")]
        public string Erid { get; set; }

        /// <summary>
        /// Дата создания
        /// </summary>
        [JsonPropertyName("created_at")]
        public DateTimeOffset? CreatedAt { get; set; }

        /// <summary>
        /// Статус обработки
        /// </summary>
        [JsonPropertyName("status")]
        public string Status { get; set; }
    }
}

