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
    }
}
