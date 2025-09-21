using System.Text.Json.Serialization;

namespace VkOrdApiWrapper.Models.VkOrd
{
    /// <summary>
    /// Ответ VK ОРД
    /// </summary>
    public class VkOrdResponse<T>
    {
        /// <summary>
        /// Данные
        /// </summary>
        [JsonPropertyName("data")]
        public T Data { get; set; }

        /// <summary>
        /// Marker
        /// </summary>
        [JsonPropertyName("erid")]
        public string Erid { get; set; } // ERID токен

        /// <summary>
        /// Ошибка
        /// </summary>
        [JsonPropertyName("error")]
        public string Error { get; set; }

        /// <summary>
        /// Успешно ли выполнен запрос
        /// </summary>
        public bool IsSuccess => string.IsNullOrEmpty(Error);
    }

    /// <summary>
    /// Ответ при получении статуса креатива
    /// </summary>
    public class VkOrdStatusResponse
    {
        /// <summary>
        /// Статус
        /// </summary>
        [JsonPropertyName("status")]
        public string Status { get; set; } // "pending", "verified", "error"

        /// <summary>
        /// Сообщение
        /// </summary>
        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
}
