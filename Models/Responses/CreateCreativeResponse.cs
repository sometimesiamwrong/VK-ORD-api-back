using System.Text.Json.Serialization;

namespace VkOrdApiWrapper.Models.Responses
{
    /// <summary>
    /// Ответ при создании креатива
    /// </summary>
    public class CreateCreativeResponse
    {
        /// <summary>
        /// Внешний ID креатива
        /// </summary>
        public string ExternalId { get; set; }

        /// <summary>
        /// Marker
        /// </summary>
        [JsonPropertyName("erid")]
        public string Erid { get; set; } // ERID

        /// <summary>
        /// Успешно ли создан креатив
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Сообщение об ошибке
        /// </summary>
        public string ErrorMessage { get; set; }
        
        /// <summary>
        /// Время создания креатива
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
