using Refit;

namespace VkOrdApi.Person
{
    /// <summary>
    /// Клиент для работы с VK ОРД (Группа для работы с контрагентами)
    /// </summary>
    public partial interface IVkOrdApiClient
    {
        /// <summary>
        /// Получить список всех контрагентов (persons) - возвращает external_ids с пагинацией
        /// </summary>
        [Get("/v1/person")]
        Task<VkOrdPersonListResponse> GetPersonsAsync([Query] int? offset = null, [Query] int? limit = null);

        /// <summary>
        /// Создать/обновить контрагента (person)
        /// </summary>
        [Put("/v1/person/{externalId}")]
        Task CreateOrUpdatePersonAsync(
            string externalId,
            [Body] VkOrdPersonResponse personResponse);
   
        /// <summary>
        /// Получить контрагента (person) по external_id
        /// </summary>
        [Get("/v1/person/{externalId}")]
        Task<VkOrdPersonResponse> GetPersonAsync(string externalId);
    }
}
