namespace VkOrdApiWrapper.Models.Responses
{
    /// <summary>
    /// Ответ на загрузку медиа файла
    /// </summary>
    public class UploadMediaResponse : ApiResponse
    {
        /// <summary>
        /// Внешний ID медиа файла
        /// </summary>
        public string ExternalId { get; set; }

        /// <summary>
        /// ERID токен
        /// </summary>
        public string Erid { get; set; }

        /// <summary>
        /// URL загруженного файла
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Сообщение об ошибке
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}
