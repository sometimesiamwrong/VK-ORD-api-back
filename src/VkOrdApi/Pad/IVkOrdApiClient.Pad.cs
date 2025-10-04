using Refit;

namespace VkOrdApi.Pad
{
    /// <summary>
    /// Методы клиента для работы с платформами в VK ОРД
    /// </summary>
    public partial interface IVkOrdApiClient
    {
        /// <summary>
        /// Получить список всех платформ (pads) - возвращает external_ids с пагинацией
        /// </summary>
        [Get("/v1/pad")]
        Task<VkOrdPadListResponse> GetPadsAsync([Query] int? offset = null, [Query] int? limit = null);

        /// <summary>
        /// Создать/обновить платформу (pad)
        /// </summary>
        [Put("/v1/pad/{external_id}")]
        Task CreateOrUpdatePadAsync(
            [AliasAs("external_id")] string externalId,
            [Body] VkOrdCreateUpdatePadRequest padRequest);

        /// <summary>
        /// Получить платформу (pad) по external_id
        /// </summary>
        [Get("/v1/pad/{external_id}")]
        Task<VkOrdPadResponse> GetPadByExternalIdAsync([AliasAs("external_id")] string externalId);

        /// <summary>
        /// Получить ограниченную информацию о платформах
        /// </summary>
        [Get("/v1/pad/info/restricted")]
        Task<VkOrdPadRestrictedInfoResponse> GetPadRestrictedInfoAsync();
    }
}
