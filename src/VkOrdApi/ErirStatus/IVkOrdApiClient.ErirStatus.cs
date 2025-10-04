using Refit;

namespace VkOrdApi.ErirStatus
{
    /// <summary>
    /// Методы клиента для работы со статусами ЕРИР в VK ОРД
    /// </summary>
    public partial interface IVkOrdApiClient
    {
        /// <summary>
        /// Получить статус обработки конкретного объекта рекламы в ЕРИР (GET /v1/{data_type}/{external_id}/erir_status)
        /// data_type: тип объекта (e.g., 'invoice', 'creative'), external_id: уникальный ID объекта.
        /// Возвращает erir_status (processing/bad/verified), updated_by_user_ts, finalized_ts (if bad/verified), messages (if bad).
        /// </summary>
        [Get("/v1/{data_type}/{external_id}/erir_status")]
        Task<VkOrdErirStatusResponse> GetErirStatusAsync(
            [AliasAs("data_type")] string dataType,
            [AliasAs("external_id")] string externalId);

        /// <summary>
        /// Получить список статусов обработки объектов рекламы в ЕРИР (GET /v1/erir_statuses)
        /// Фильтрация через request-объект: data_type (required enum: person/contract/creative/pad/invoice/statistics/cid),
        /// erir_status (optional enum: processing/bad/verified), offset (default 0), limit (default 10000, max 60000),
        /// limit_per_entity (optional, interacts with limit), external_ids (optional array of strings).
        /// Пагинированный список с erir_status, timestamps и messages где применимо.
        /// Порядок объектов: person, contract, creative, invoice, pad, statistics.
        /// </summary>
        [Get("/v1/erir_statuses")]
        Task<VkOrdErirStatusesListResponse> GetErirStatusesAsync([Query] VkOrdErirStatusesRequest request);

        /// <summary>
        /// Получить статусы обработки нескольких объектов рекламы в ЕРИР (POST /v1/erir_statuses)
        /// Body: запрос с фильтрами (data_type required, erir_status optional, offset=0, limit=10000, limit_per_entity optional, external_ids optional).
        /// Возвращает пагинированный список статусов. Аналогично GET, но для batch-запросов.
        /// </summary>
        [Post("/v1/erir_statuses")]
        Task<VkOrdErirStatusesListResponse> GetErirStatusesBatchAsync([Body] VkOrdErirStatusesRequest request);
    }
}
