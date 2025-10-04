using Refit;

namespace VkOrdApi.Creative
{
    /// <summary>
    /// Методы клиента для работы с креативами в VK ОРД
    /// </summary>
    public partial interface IVkOrdApiClient
    {
        /// <summary>
        /// Получить список всех креативов - возвращает external_ids с пагинацией (v1)
        /// </summary>
        [Get("/v1/creative")]
        Task<VkOrdCreativeListResponse> GetCreativesV1Async([Query] int? offset = null, [Query] int? limit = null);

        /// <summary>
        /// Получить список маркеров рекламы (ERIDs) (v1)
        /// </summary>
        [Get("/v1/creative/list/erids")]
        Task<VkOrdEridsListResponse> GetEridsListV1Async([Query] int? offset = null, [Query] int? limit = null);

        /// <summary>
        /// Получить список пар маркеров рекламы и внешних идентификаторов (v1)
        /// </summary>
        [Get("/v1/creative/list/erid_external_ids")]
        Task<VkOrdEridExternalIdsListResponse> GetEridExternalIdsListV1Async([Query] int? offset = null, [Query] int? limit = null);

        /// <summary>
        /// Создать/обновить креатив (v2)
        /// </summary>
        [Put("/v2/creative/{external_id}")]
        Task CreateOrUpdateCreativeV2Async(
            [AliasAs("external_id")] string externalId,
            [Body] VkOrdCreativeV2Request creativeRequest);

        /// <summary>
        /// Получить креатив (v2)
        /// </summary>
        [Get("/v2/creative/{external_id}")]
        Task<VkOrdCreativeV2Response> GetCreativeV2ByExternalIdAsync([AliasAs("external_id")] string externalId);

        /// <summary>
        /// Получить креатив по маркеру рекламы (ERID) (v2)
        /// </summary>
        [Get("/v2/creative/by_erid/{erid}")]
        Task<VkOrdCreativeV2Response> GetCreativeV2ByEridAsync([AliasAs("erid")] string erid);

        /// <summary>
        /// Создать/обновить креатив (v3)
        /// </summary>
        [Put("/v3/creative/{external_id}")]
        Task CreateOrUpdateCreativeV3Async(
            [AliasAs("external_id")] string externalId,
            [Body] VkOrdCreativeV3Request creativeRequest);

        /// <summary>
        /// Получить креатив (v3)
        /// </summary>
        [Get("/v3/creative/{external_id}")]
        Task<VkOrdCreativeV3Response> GetCreativeV3ByExternalIdAsync([AliasAs("external_id")] string externalId);

        /// <summary>
        /// Получить креатив по маркеру рекламы (ERID) (v3)
        /// </summary>
        [Get("/v3/creative/by_erid/{erid}")]
        Task<VkOrdCreativeV3Response> GetCreativeV3ByEridAsync([AliasAs("erid")] string erid);

        /// <summary>
        /// Добавить тексты в креатив (v1)
        /// </summary>
        [Post("/v1/creative/{external_id}/add_text")]
        Task AddTextToCreativeV1Async(
            [AliasAs("external_id")] string externalId,
            [Body] VkOrdAddTextRequest addTextRequest);

        /// <summary>
        /// Добавить внешние ресурсы в креатив (v1)
        /// </summary>
        [Post("/v1/creative/{external_id}/add_external_media")]
        Task AddExternalMediaToCreativeV1Async(
            [AliasAs("external_id")] string externalId,
            [Body] VkOrdAddExternalMediaRequest addExternalMediaRequest);

        /// <summary>
        /// Добавить медиафайлы в креатив (v1)
        /// </summary>
        [Post("/v1/creative/{external_id}/add_media")]
        Task AddMediaToCreativeV1Async(
            [AliasAs("external_id")] string externalId,
            [Body] VkOrdAddMediaRequest addMediaRequest);
    }
}
