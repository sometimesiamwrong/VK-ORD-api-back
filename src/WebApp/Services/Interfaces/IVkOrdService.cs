using Domain;
using VkOrdApi.Creative;
using VkOrdApi.Media;
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
    Task CreateOrUpdateContract(CreateContractRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Получить информацию о контракте по external_id
    /// </summary>
    Task<ContractResponse> GetContract(string externalId, CancellationToken cancellationToken);

    /// <summary>
    /// Получить список контрактов
    /// </summary>
    Task<GetContractResponseDto> GetPageContract(PageRequest pageRequest, CancellationToken cancellationToken);

    #endregion

    #region Креативы

    /// <summary>
    /// Создать креатив
    /// </summary>
    Task<VkOrdCreativeV3RequestResponse> CreateCreative(CreateCreativeRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Получить информацию о креативе по external_id
    /// </summary>
    Task<VkOrdCreativeV3Response> GetCreative(string externalId, CancellationToken cancellationToken);

    /// <summary>
    /// Получить список креативов с детальными данными
    /// </summary>
    Task<GetCreativesResponse> GetPageCreatives(PageRequest pageRequest, CancellationToken cancellationToken);

    /// <summary>
    /// Получить креатив по ERID
    /// </summary>
    Task<VkOrdCreativeV3Response> GetCreativeByErid(string erid, CancellationToken cancellationToken);
    
    #endregion

    #region Контрагенты

    /// <summary>
    /// Создать контрагента в VK ОРД из данных DaData по ИНН
    /// </summary>
    Task CreateCounterpartyFromInn(string inn, List<VkOrdPersonRoles> types, CancellationToken cancellationToken);

    /// <summary>
    /// Получить список всех контрагентов с полными данными из VK ОРД
    /// </summary>
    Task<GetCounterpartiesResponseDto> GetPageCounterparties(PageRequest pageRequest, CancellationToken cancellationToken);

    /// <summary>
    /// Получить контрагента по external_id из VK ОРД
    /// </summary>
    Task<GetCounterpartyResponse?> GetCounterpartyById(string externalId, CancellationToken cancellationToken);

    #endregion

    #region Медиа файлы

    /// <summary>
    /// Загрузить медиа файл
    /// </summary>
    Task UploadMedia(UploadMediaRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Получить информацию о медиа файле
    /// </summary>
    Task<VkOrdMediaInfoResponse> GetMedia(string externalId, CancellationToken cancellationToken);

    /// <summary>
    /// Получить список медиа файлов
    /// </summary>
    Task<VkOrdMediaInfoListResponseDto> GetPageMedia(PageRequest pageRequest, CancellationToken cancellationToken);
    
    #endregion
}
