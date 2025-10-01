using System.Text.Json.Serialization;

namespace VkOrdApiWrapper.Models.VkOrd
{
    /// <summary>
    /// Креатив VK ОРД
    /// </summary>
    public class VkOrdCreative
    {
        /// <summary>
        /// Внешний ID креатива
        /// </summary>
        [JsonPropertyName("external_id")]
        public string ExternalId { get; set; }

        /// <summary>
        /// Внешний ID контракта
        /// </summary>
        [JsonPropertyName("contract_external_ids")]
        public List<string> ContractExternalIds { get; set; }

        /// <summary>
        /// Внешний ID медиа
        /// </summary>
        [JsonPropertyName("media_external_ids")]
        public List<string> MediaExternalIds { get; set; }

        /// <summary>
        /// Название креатива
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// ИНН рекламодателя
        /// </summary>
        [JsonPropertyName("kktus")]
        public List<string> KKTYCodes { get; set; }

        /// <summary>
        /// Тип оплаты
        /// </summary>
        [JsonPropertyName("pay_type")]
        public string PayType { get; set; }

        /// <summary>
        /// Формат
        /// </summary>
        [JsonPropertyName("form")]
        public string Form { get; set; }

        /// <summary>
        /// URL контента
        /// </summary>
        [JsonPropertyName("target_urls")]
        public List<string> TargetUrls { get; set; }

        /// <summary>
        /// Целевая аудитория
        /// </summary>
        [JsonPropertyName("targeting")]
        public string Targeting { get; set; }

        /// <summary>
        /// Текст креатива
        /// </summary>
        [JsonPropertyName("texts")]
        public List<string> Texts { get; set; }

        /// <summary>
        /// Флаги
        /// </summary>
        [JsonPropertyName("flags")]
        public List<string> Flags { get; set; }
    }
}
