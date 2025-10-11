using System.ComponentModel.DataAnnotations;
using MediatR;
using VkOrdApi.Creative;
using VkOrdApi.Statistics;

namespace WebApp.Models.Requests
{
    /// <summary>
    /// Запрос на создание креатива
    /// </summary>
    public class CreateCreativeRequest : IRequest<VkOrdCreativeV3RequestResponse>
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
        public List<string> Kktus { get; set; } = new();

        /// <summary>
        /// Формат
        /// </summary>
        [Required]
        public VkOrdCreativeForm Type { get; set; }

        /// <summary>
        /// Целевые URL
        /// </summary>
        public List<string> TargetUrls { get; set; } = new();

        /// <summary>
        /// Целевая аудитория
        /// </summary>
        public string TargetAudience { get; set; } = string.Empty;

        /// <summary>
        /// Тексты креатива
        /// </summary>
        public List<string> Texts { get; set; } = new List<string>();
        
        /// <summary>
        /// Название креатива
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Внешний ID персоны
        /// </summary>
        public string? PersonExternalId { get; set; }

        /// <summary>
        /// Бренд
        /// </summary>
        public string? Brand { get; set; }

        /// <summary>
        /// Категория
        /// </summary>
        public string? Category { get; set; }

        /// <summary>
        /// Описание
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Тип оплаты
        /// </summary>
        public VkOrdPayType PayType { get; set; }

        /// <summary>
        /// Форма
        /// </summary>
        public string? Form { get; set; }

        /// <summary>
        /// Флаги
        /// </summary>
        public List<VkOrdCreativeFlag> Flags { get; set; } = new List<VkOrdCreativeFlag>();
    }
}
