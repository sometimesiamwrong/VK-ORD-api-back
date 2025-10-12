using System.ComponentModel.DataAnnotations;
using Domain.Entities.Enums.VkOrd;
using Domain.VkOrdApi.Creative;
using Domain.VkOrdApi.Statistics;
using MediatR;

namespace WebApp.Models.Requests
{
    /// <summary>
    /// Запрос на создание креатива
    /// </summary>
    public class CreateCreativeRequest : IRequest<VkOrdApiCreativeV3RequestResponse>
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
        public VkOrdApiCreativeForm Type { get; set; }

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
        public VkOrdApiPayType ApiPayType { get; set; }

        /// <summary>
        /// Форма
        /// </summary>
        public string? Form { get; set; }

        /// <summary>
        /// Флаги
        /// </summary>
        public List<VkOrdApiCreativeFlag> Flags { get; set; } = new List<VkOrdApiCreativeFlag>();
    }
}
