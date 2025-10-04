using Refit;

namespace VkOrdApi.Invoice
{
    /// <summary>
    /// Методы клиента для работы с актами в VK ОРД
    /// </summary>
    public partial interface IVkOrdApiClient
    {
        /// <summary>
        /// Получить список всех актов - возвращает external_ids с пагинацией (v1)
        /// </summary>
        [Get("/v1/invoice")]
        Task<VkOrdInvoiceListResponse> GetInvoicesV1Async([Query] int? offset = null, [Query] int? limit = null);

        /// <summary>
        /// Удалить договоры из акта (v2)
        /// </summary>
        [Post("/v2/invoice/{external_id}/delete")]
        Task DeleteContractsFromInvoiceV2Async(
            [AliasAs("external_id")] string externalId,
            [Body] VkOrdDeleteContractsFromInvoiceRequest deleteRequest);

        /// <summary>
        /// Отправить акт в ЕРИР (v2)
        /// </summary>
        [Post("/v2/invoice/{external_id}/ready")]
        Task SendInvoiceToErirV2Async(
            [AliasAs("external_id")] string externalId,
            [Body] VkOrdSendInvoiceToErirRequest sendRequest);

        /// <summary>
        /// Создать полный акт (v3, detailed schema)
        /// </summary>
        [Put("/v3/invoice/{external_id}")]
        Task CreateFullInvoiceV3Async(
            [AliasAs("external_id")] string externalId,
            [Body] VkOrdFullInvoiceRequest fullRequest);

        /// <summary>
        /// Получить полный акт (v3, detailed schema)
        /// </summary>
        [Get("/v3/invoice/{external_id}")]
        Task<VkOrdFullInvoiceResponse> GetFullInvoiceV3Async([AliasAs("external_id")] string externalId);

        /// <summary>
        /// Удалить акт (v3)
        /// </summary>
        [Delete("/v3/invoice/{external_id}")]
        Task DeleteInvoiceV3Async([AliasAs("external_id")] string externalId);

        /// <summary>
        /// Создать заголовок акта (v3)
        /// </summary>
        [Put("/v3/invoice/{external_id}/header")]
        Task CreateInvoiceHeaderV3Async(
            [AliasAs("external_id")] string externalId,
            [Body] VkOrdInvoiceHeaderRequest headerRequest);

        /// <summary>
        /// Получить заголовок акта без разаллокаций (v3)
        /// </summary>
        [Get("/v3/invoice/{external_id}/header")]
        Task<VkOrdInvoiceHeaderResponse> GetInvoiceHeaderV3Async([AliasAs("external_id")] string externalId);

        /// <summary>
        /// Добавить договоры в акт (v3)
        /// </summary>
        [Patch("/v3/invoice/{external_id}/items")]
        Task AddContractsToInvoiceV3Async(
            [AliasAs("external_id")] string externalId,
            [Body] VkOrdAddContractsToInvoiceRequest addRequest);
    }
}
