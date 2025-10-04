using Refit;

namespace VkOrdApi.Contract
{
    /// <summary>
    /// Методы клиента для работы с договорами в VK ОРД
    /// </summary>
    public partial interface IVkOrdApiClient
    {
        /// <summary>
        /// Получить список всех договоров (contracts) - возвращает external_ids с пагинацией
        /// </summary>
        [Get("/v1/contract")]
        Task<VkOrdContractListResponse> GetContractsAsync([Query] int? offset = null, [Query] int? limit = null);

        /// <summary>
        /// Создать/обновить договор (contract)
        /// </summary>
        [Put("/v1/contract/{external_id}")]
        Task CreateOrUpdateContractAsync(
            [AliasAs("external_id")] string externalId,
            [Body] VkOrdCreateUpdateContractRequest contractRequest,
            [Query] bool? updateAdditionalContractsParties = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить договор (contract) по external_id
        /// </summary>
        [Get("/v1/contract/{external_id}")]
        Task<VkOrdContractResponse> GetContractByExternalIdAsync([AliasAs("external_id")] string externalId);

        /// <summary>
        /// Запросить CID для контракта
        /// </summary>
        [Post("/v1/contract/{external_id}/create_cid")]
        Task RequestContractCidAsync([AliasAs("external_id")] string externalId);
    }
}
