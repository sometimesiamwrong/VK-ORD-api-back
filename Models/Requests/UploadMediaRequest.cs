using System.ComponentModel.DataAnnotations;

namespace VkOrdApiWrapper.Models.Requests
{
    /// <summary>
    /// Запрос на загрузку медиа файла
    /// </summary>
    public class UploadMediaRequest : AuthorizedRequestBase
    {
        /// <summary>
        /// Внешний ID медиа файла
        /// </summary>
        [Required]
        public string ExternalId { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Файл для загрузки
        /// </summary>
        [Required]
        public Stream FileStream { get; set; }

        /// <summary>
        /// Имя файла
        /// </summary>
        [Required]
        public string FileName { get; set; }

        /// <summary>
        /// MIME тип файла
        /// </summary>
        public string ContentType { get; set; }
    }
}

