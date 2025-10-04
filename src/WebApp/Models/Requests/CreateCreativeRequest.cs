using System.ComponentModel.DataAnnotations;
using MediatR;
using VkOrdApi.Creative;

namespace WebApp.Models.Requests
{
    /// <summary>
    /// Запрос на создание креатива
    /// </summary>
    public class CreateCreativeRequest : IRequestWithVkOrdKey, IRequest
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
        public List<string> ContractExternalIds { get; set; } = new();

        /// <summary>
        /// Внешний ID медиа
        /// </summary>
        public List<string> MediaExternalIds { get; set; } = new List<string>();

        /// <summary>
        /// Код ККТУ
        /// </summary>
        [Required]
        public List<string> KKTYCodes { get; set; } = new();

        /// <summary>
        /// Формат
        /// </summary>
        [Required]
        public VkOrdCreativeType Format { get; set; }

        /// <summary>
        /// URL контента
        /// </summary>
        public List<string> ContentUrls { get; set; } = new();

        /// <summary>
        /// Целевая аудитория
        /// </summary>
        public string TargetAudience { get; set; } = string.Empty;

        /// <summary>
        /// Текст креатива
        /// </summary>
        public string Text { get; set; } = string.Empty;
        
        /// <summary>
        /// Название креатива
        /// </summary>
        public string Name { get; set; } = string.Empty;
    }
}
