using Refit;

namespace VkOrdApi.Media
{
    /// <summary>
    /// Методы клиента для работы с медиафайлами в VK ОРД
    /// </summary>
    public partial interface IVkOrdApiClient
    {
        /// <summary>
        /// Получить список всех медиафайлов - возвращает external_ids с пагинацией
        /// </summary>
        [Get("/v1/media")]
        Task<VkOrdMediaListResponse> GetMediasAsync([Query] int? offset = null, [Query] int? limit = null);

        /// <summary>
        /// Загрузить медиафайл (multipart/form-data, поле 'file' для binary)
        /// </summary>
        [Multipart]
        [Put("/v1/media/{external_id}")]
        Task UploadMediaAsync(
            [AliasAs("external_id")] string externalId,
            [AliasAs("file")] StreamPart file);

        /// <summary>
        /// Получить бинарный медиафайл
        /// </summary>
        [Get("/v1/media/{external_id}")]
        Task<byte[]> GetMediaFileAsync([AliasAs("external_id")] string externalId);

        /// <summary>
        /// Получить данные медиафайла (info)
        /// </summary>
        [Get("/v1/media/{external_id}/info")]
        Task<VkOrdMediaInfoResponse> GetMediaInfoAsync([AliasAs("external_id")] string externalId);
    }
}
