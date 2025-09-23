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
        Task<CreateContractResponse> CreateOrUpdateContractAsync(CreateContractRequest request);

        /// <summary>
        /// Получить информацию о контракте по external_id
        /// </summary>
        Task<ContractResponse> GetContractAsync(string externalId);

        #endregion

        #region Креативы

        /// <summary>
        /// Создать креатив
        /// </summary>
        Task<CreateCreativeResponse> CreateCreativeAsync(CreateCreativeRequest request);

        /// <summary>
        /// Получить информацию о креативе по external_id
        /// </summary>
        Task<CreateCreativeResponse> GetCreativeAsync(string externalId);

        /// <summary>
        /// Получить статус креатива
        /// </summary>
        Task<VkOrdStatusResponse> GetCreativeStatusAsync(string externalId);

        /// <summary>
        /// Удалить креатив
        /// </summary>
        Task<bool> DeleteCreativeAsync(string externalId);

        /// <summary>
        /// Создать пакет креативов
        /// </summary>
        Task<List<CreateCreativeResponse>> CreateBulkCreativesAsync(List<CreateCreativeRequest> requests);

        /// <summary>
        /// Проверить, что креатив прошел верификацию в ЕРИР
        /// </summary>
        Task<bool> IsCreativeVerifiedAsync(string externalId, int maxWaitTimeMinutes = 120);

        #endregion

        #region Контрагенты

        /// <summary>
        /// Создать контрагента в VK ОРД из данных DaData по ИНН
        /// </summary>
        Task<StatusResponse> CreateCounterpartyFromInnAsync(string inn, List<string> types);

        #endregion
    }
}
