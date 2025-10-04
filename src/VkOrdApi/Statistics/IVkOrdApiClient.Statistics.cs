using Refit;

namespace VkOrdApi.Statistics
{
    /// <summary>
    /// Методы клиента для работы со статистикой в VK ОРД
    /// </summary>
    public partial interface IVkOrdApiClient
    {
        /// <summary>
        /// Получить список статистик (v2, filters: creative_external_id?, pad_external_id?, date_start_actual_from?, date_start_actual_to?, offset?, limit?)
        /// </summary>
        [Get("/v2/statistics/list")]
        Task<VkOrdStatisticsListResponse> GetStatisticsListV2Async([Query] VkOrdStatisticsListRequest request);

        /// <summary>
        /// Создать статистику (batch POST v2, body array items)
        /// </summary>
        [Post("/v2/statistics")]
        Task CreateStatisticsV2Async([Body(BodySerializationMethod.Serialized)] List<VkOrdStatisticsItem> items);

        /// <summary>
        /// Удалить статистику (POST v1, body external_ids list)
        /// </summary>
        [Post("/v1/statistics/delete")]
        Task DeleteStatisticsV1Async([Body] VkOrdDeleteStatisticsRequest deleteRequest);
    }
}
