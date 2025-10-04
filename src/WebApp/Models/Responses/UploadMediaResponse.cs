namespace WebApp.Models.Responses
{
    /// <summary>
    /// Ответ на загрузку медиа файла
    /// </summary>
    public class UploadMediaResponse
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
    }
}
