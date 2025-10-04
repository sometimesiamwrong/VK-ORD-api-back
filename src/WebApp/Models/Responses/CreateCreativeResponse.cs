using System.Text.Json.Serialization;

namespace WebApp.Models.Responses
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
        /// Время создания креатива
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
