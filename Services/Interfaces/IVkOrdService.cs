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
        Task<CreateContractResponse> CreateOrUpdateContractAsync(CreateContractRequest request, Guid userId, string? environment = null);

        /// <summary>
        /// Получить информацию о контракте по external_id
        /// </summary>
        Task<ContractResponse> GetContractAsync(string externalId, Guid userId, string? environment = null);

        #endregion

        #region Креативы

        /// <summary>
        /// Создать креатив
        /// </summary>
        Task<CreateCreativeResponse> CreateCreativeAsync(CreateCreativeRequest request, Guid userId, string? environment = null);

        /// <summary>
        /// Получить информацию о креативе по external_id
        /// </summary>
        Task<CreateCreativeResponse> GetCreativeAsync(string externalId, Guid userId, string? environment = null);

        /// <summary>
        /// Получить статус креатива
        /// </summary>
        Task<VkOrdStatusResponse> GetCreativeStatusAsync(string externalId, Guid userId, string? environment = null);

        /// <summary>
        /// Удалить креатив
        /// </summary>
        Task<bool> DeleteCreativeAsync(string externalId, Guid userId, string? environment = null);

        /// <summary>
        /// Создать пакет креативов
        /// </summary>
        Task<List<CreateCreativeResponse>> CreateBulkCreativesAsync(List<CreateCreativeRequest> requests, Guid userId, string? environment = null);

        /// <summary>
        /// Проверить, что креатив прошел верификацию в ЕРИР
        /// </summary>
        Task<bool> IsCreativeVerifiedAsync(string externalId, Guid userId, string? environment = null, int maxWaitTimeMinutes = 120);

        #endregion

        #region Контрагенты

        /// <summary>
        /// Создать контрагента в VK ОРД из данных DaData по ИНН
        /// </summary>
        Task<StatusResponse> CreateCounterpartyFromInnAsync(string inn, List<string> types, Guid userId, string? environment = null);

        /// <summary>
        /// Получить список всех контрагентов с полными данными из VK ОРД
        /// </summary>
        Task<GetCounterpartiesResponse> GetAllCounterpartiesAsync(Guid userId, string? environment = null, int? offset = null, int? limit = null);

        /// <summary>
        /// Получить контрагента по external_id из VK ОРД
        /// </summary>
        Task<GetCounterpartyResponse> GetCounterpartyByIdAsync(string externalId, Guid userId, string? environment = null);

        #endregion

        #region Медиа файлы

        /// <summary>
        /// Загрузить медиа файл
        /// </summary>
        Task<UploadMediaResponse> UploadMediaAsync(UploadMediaRequest request, Guid userId, string? environment = null);

        /// <summary>
        /// Получить информацию о медиа файле
        /// </summary>
        Task<GetMediaResponse> GetMediaAsync(string externalId, Guid userId, string? environment = null);

        /// <summary>
        /// Удалить медиа файл
        /// </summary>
        Task<bool> DeleteMediaAsync(string externalId, Guid userId, string? environment = null);

        #endregion
    }
}
