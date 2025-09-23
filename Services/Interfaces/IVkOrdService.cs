using VkOrdApiWrapper.Models.Requests;
using VkOrdApiWrapper.Models.Responses;
using VkOrdApiWrapper.Models.VkOrd;

namespace VkOrdApiWrapper.Services.Interfaces
{
    /// <summary>
    /// Сервис для работы с VK ОРД API
    /// </summary>
    public interface IVkOrdService
    {
        #region Контракты

        /// <summary>
        /// Создать или обновить контракт
        /// </summary>
        Task<CreateContractResponse> CreateOrUpdateContractAsync(CreateContractRequest request, VkApiContext apiContext);

        /// <summary>
        /// Получить информацию о контракте по external_id
        /// </summary>
        Task<ContractResponse> GetContractAsync(string externalId, VkApiContext apiContext);

        #endregion

        #region Креативы

        /// <summary>
        /// Создать креатив
        /// </summary>
        Task<CreateCreativeResponse> CreateCreativeAsync(CreateCreativeRequest request, VkApiContext apiContext);

        /// <summary>
        /// Получить информацию о креативе по external_id
        /// </summary>
        Task<CreateCreativeResponse> GetCreativeAsync(string externalId, VkApiContext apiContext);

        /// <summary>
        /// Получить статус креатива
        /// </summary>
        Task<VkOrdStatusResponse> GetCreativeStatusAsync(string externalId, VkApiContext apiContext);

        /// <summary>
        /// Удалить креатив
        /// </summary>
        Task<bool> DeleteCreativeAsync(string externalId, VkApiContext apiContext);

        /// <summary>
        /// Создать пакет креативов
        /// </summary>
        Task<List<CreateCreativeResponse>> CreateBulkCreativesAsync(List<CreateCreativeRequest> requests, VkApiContext apiContext);

        /// <summary>
        /// Проверить, что креатив прошел верификацию в ЕРИР
        /// </summary>
        Task<bool> IsCreativeVerifiedAsync(string externalId, VkApiContext apiContext, int maxWaitTimeMinutes = 120);

        #endregion

        #region Контрагенты

        /// <summary>
        /// Создать контрагента в VK ОРД из данных DaData по ИНН
        /// </summary>
        Task<StatusResponse> CreateCounterpartyFromInnAsync(string inn, List<string> types, VkApiContext apiContext);

        #endregion
    }
}
