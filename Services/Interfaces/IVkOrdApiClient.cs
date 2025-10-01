using Refit;
using VkOrdApiWrapper.Models.VkOrd;

namespace VkOrdApiWrapper.Services.Interfaces
{
    /// <summary>
    /// Клиент для работы с VK ОРД
    /// </summary>
    public interface IVkOrdApiClient
    {
        /// <summary>
        /// Создать или обновить контракт
        /// </summary>
        [Put("/v1/contract/{externalId}")]
        Task<VkOrdResponse<VkOrdContract>> CreateOrUpdateContractAsync(
            string externalId,
            [Body] VkOrdContract contract);

        /// <summary>
        /// Получить контракт
        /// </summary>
        [Get("/v1/contract/{externalId}")]
        Task<VkOrdResponse<VkOrdContract>> GetContractAsync(
            string externalId);

        /// <summary>
        /// Создать или обновить креатив
        /// </summary>
        [Put("/v3/creative/{externalId}")]
        Task<VkOrdResponse<VkOrdCreative>> CreateOrUpdateCreativeAsync(
            string externalId,
            [Body] VkOrdCreative creative);

        /// <summary>
        /// Получить креатив
        /// </summary>
        [Get("/v3/creative/{externalId}")]
        Task<VkOrdResponse<VkOrdCreative>> GetCreativeAsync(
            string externalId);

        /// <summary>
        /// Получить статус креатива
        /// </summary>
        [Get("/v1/status/creative/{externalId}")]
        Task<VkOrdStatusResponse> GetCreativeStatusAsync(
            string externalId);

        /// <summary>
        /// Удалить креатив
        /// </summary>
        [Delete("/v1/creative/{externalId}")]
        Task<ApiResponse<object>> DeleteCreativeAsync(
            string externalId);

        /// <summary>
        /// Создать/обновить контрагента (person)
        /// </summary>
        [Put("/v1/person/{externalId}")]
        Task<VkOrdResponse<object>> CreateOrUpdatePersonAsync(
            string externalId,
            [Body] VkOrdPerson person);

        /// <summary>
        /// Получить список всех контрагентов (persons) - возвращает external_ids с пагинацией
        /// </summary>
        [Get("/v1/person")]
        Task<VkOrdPersonListResponse> GetPersonsAsync([Query] int? offset = null, [Query] int? limit = null);

        /// <summary>
        /// Получить контрагента (person) по external_id
        /// </summary>
        [Get("/v1/person/{externalId}")]
        Task<VkOrdPerson> GetPersonAsync(string externalId);

        #region Медиа файлы

        /// <summary>
        /// Загрузить медиа файл
        /// </summary>
        [Put("/v1/media/{externalId}")]
        [Multipart]
        Task<VkOrdResponse<VkOrdMedia>> UploadMediaAsync(
            string externalId,
            [AliasAs("file")] StreamPart file);

        /// <summary>
        /// Получить информацию о медиа файле
        /// </summary>
        [Get("/v1/media/{externalId}")]
        Task<VkOrdResponse<VkOrdMedia>> GetMediaAsync(
            string externalId);

        /// <summary>
        /// Удалить медиа файл
        /// </summary>
        [Delete("/v1/media/{externalId}")]
        Task<ApiResponse<object>> DeleteMediaAsync(
            string externalId);

        #endregion
    }
}
