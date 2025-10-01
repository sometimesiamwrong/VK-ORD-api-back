using VkOrdApiWrapper.Models.VkOrd;

namespace VkOrdApiWrapper.Models.Responses
{
    /// <summary>
    /// Ответ на получение информации о медиа файле
    /// </summary>
    public class GetMediaResponse : ApiResponse
    {
        /// <summary>
        /// Внешний ID медиа файла
        /// </summary>
        public string ExternalId { get; set; }

        /// <summary>
        /// Информация о медиа файле
        /// </summary>
        public VkOrdMedia Media { get; set; }

        /// <summary>
        /// Сообщение об ошибке
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}
