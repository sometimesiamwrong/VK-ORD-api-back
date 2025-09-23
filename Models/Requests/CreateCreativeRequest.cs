using System.ComponentModel.DataAnnotations;
using VkOrdApiWrapper.Models.VkOrd;

namespace VkOrdApiWrapper.Models.Requests
{
    /// <summary>
    /// Запрос на создание креатива
    /// </summary>
    public class CreateCreativeRequest
    {
        /// <summary>
        /// Внешний ID креатива
        /// </summary>
        [Required]
        public string ExternalId { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();

        /// <summary>
        /// Внешний ID контракта
        /// </summary>
        [Required]
        public List<string> ContractExternalIds { get; set; }

        /// <summary>
        /// Код ККТУ
        /// </summary>
        [Required]
        public List<string> KKTYCodes { get; set; }

        /// <summary>
        /// Формат
        /// </summary>
        [Required]
        public VkCreativeForm Format { get; set; }

        /// <summary>
        /// URL контента
        /// </summary>
        public List<string> ContentUrls { get; set; }

        /// <summary>
        /// Целевая аудитория
        /// </summary>
        public string TargetAudience { get; set; }

        /// <summary>
        /// Текст креатива
        /// </summary>
        public string Text { get; set; }
        
        /// <summary>
        /// Название креатива
        /// </summary>
        public string Name { get; set; }
    }
}
