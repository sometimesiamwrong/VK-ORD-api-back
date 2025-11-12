using Domain.VkOrdApi.Creative;

namespace Domain.Models.Responses
{
    /// <summary>
    /// Ответ при получении креатива по external ID
    /// </summary>
    public class GetCreativeResponse
    {
        /// <summary>
        /// Внешний ID креатива
        /// </summary>
        public string ExternalId { get; set; } = string.Empty;

        /// <summary>
        /// Данные креатива
        /// </summary>
        public VkOrdApiCreativeV3Response Data { get; set; }
    }

    /// <summary>
    /// Ответ при получении креатива по ERID
    /// </summary>
    public class GetCreativeByEridResponse
    {
        /// <summary>
        /// ERID креатива
        /// </summary>
        public string Erid { get; set; } = string.Empty;

        /// <summary>
        /// Данные креатива
        /// </summary>
        public VkOrdApiCreativeV3Response Data { get; set; }
    }
}
