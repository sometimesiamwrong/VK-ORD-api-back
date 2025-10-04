using VkOrdApi.Creative;
using VkOrdApi.Person;
using WebApp.Models.Requests;
using WebApp.Models.Responses;

namespace WebApp.Services.Interfaces;

/// <summary>
/// Сервис для работы с VK ОРД API
/// </summary>
public interface IVkOrdService
{
    #region Контракты

    /// <summary>
    /// Создать или обновить контракт
    /// </summary>
    Task CreateOrUpdateContractAsync(CreateContractRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Получить информацию о контракте по external_id
    /// </summary>
    Task<ContractResponse> GetContractAsync(string externalId, long userId, CancellationToken cancellationToken);

    #endregion

    #region Креативы

    /// <summary>
    /// Создать креатив
    /// </summary>
    Task<CreateCreativeResponse> CreateCreativeAsync(CreateCreativeRequest request, long userId, CancellationToken cancellationToken);

    /// <summary>
    /// Получить информацию о креативе по external_id
    /// </summary>
    Task<CreateCreativeResponse> GetCreativeAsync(string externalId, long userId, CancellationToken cancellationToken);

    /// <summary>
    /// Получить список креативов с детальными данными
    /// </summary>
    Task<GetCreativesResponse> GetAllCreativesAsync(long userId, int? offset = null, int? limit = null, CancellationToken cancellationToken);

    /// <summary>
    /// Получить креатив по ERID
    /// </summary>
    Task<CreativeResponse> GetCreativeByEridAsync(string erid, long userId, CancellationToken cancellationToken);

    /// <summary>
    /// Получить статус креатива
    /// </summary>
    Task<VkOrdCreativeStatus> GetCreativeStatusAsync(string externalId, long userId, CancellationToken cancellationToken);

    /// <summary>
    /// Удалить креатив
    /// </summary>
    Task<bool> DeleteCreativeAsync(string externalId, long userId, CancellationToken cancellationToken);

    /// <summary>
    /// Создать пакет креативов
    /// </summary>
    Task<List<CreateCreativeResponse>> CreateBulkCreativesAsync(List<CreateCreativeRequest> requests, long userId, CancellationToken cancellationToken);

    /// <summary>
    /// Проверить, что креатив прошел верификацию в ЕРИР
    /// </summary>
    Task<bool> IsCreativeVerifiedAsync(string externalId, long userId, CancellationToken cancellationToken, int maxWaitTimeMinutes = 120);

    #endregion

    #region Контрагенты

    /// <summary>
    /// Создать контрагента в VK ОРД из данных DaData по ИНН
    /// </summary>
    Task CreateCounterpartyFromInnAsync(string inn, List<VkOrdPersonRoles> types, CancellationToken cancellationToken);

    /// <summary>
    /// Получить список всех контрагентов с полными данными из VK ОРД
    /// </summary>
    Task<GetCounterpartiesResponse> GetAllCounterpartiesAsync(long userId, CancellationToken cancellationToken, int? offset = null, int? limit = null);

    /// <summary>
    /// Получить контрагента по external_id из VK ОРД
    /// </summary>
    Task<GetCounterpartyResponse> GetCounterpartyByIdAsync(string externalId, long userId, CancellationToken cancellationToken);

    #endregion

    #region Медиа файлы

    /// <summary>
    /// Загрузить медиа файл
    /// </summary>
    Task<UploadMediaResponse> UploadMediaAsync(UploadMediaRequest request, long userId, CancellationToken cancellationToken);

    /// <summary>
    /// Получить информацию о медиа файле
    /// </summary>
    Task<GetMediaResponse> GetMediaAsync(string externalId, long userId, CancellationToken cancellationToken);

    /// <summary>
    /// Удалить медиа файл
    /// </summary>
    Task<bool> DeleteMediaAsync(string externalId, long userId, CancellationToken cancellationToken);
    
    #endregion
}
