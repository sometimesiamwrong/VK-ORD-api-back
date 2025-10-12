using Domain;
using Domain.Entities.Enums.VkOrd;
using Domain.Entities.VkOrd;
using Domain.VkOrdApi.Creative;
using Domain.VkOrdApi.Media;
using Domain.Entities.VkOrd;
using WebApp.Models.Requests;
using WebApp.Models.Responses;
using WebApp.Models.Counterparties;
using WebApp.Models.Contracts;
using WebApp.Models.Statistics;
using WebApp.Models.Common;

namespace WebApp.Services.Interfaces;

/// <summary>
/// Сервис для работы с VK ОРД API
/// </summary>
public interface IVkOrdService
{
    /// <summary>
    /// Создать или обновить контракт
    /// </summary>
    Task CreateOrUpdateContract(CreateContractRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Получить информацию о контракте по external_id
    /// </summary>
    Task<VkOrdContract> GetContract(string externalId, CancellationToken cancellationToken);

    /// <summary>
    /// Получить список контрактов
    /// </summary>
    Task<GetContractsDto> GetPageContract(PageRequest pageRequest, CancellationToken cancellationToken);

    /// <summary>
    /// Создать креатив
    /// </summary>
    Task<VkOrdApiCreativeV3RequestResponse> CreateCreative(CreateCreativeRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Получить информацию о креативе по external_id
    /// </summary>
    Task<VkOrdApiCreativeV3Response> GetCreative(string externalId, CancellationToken cancellationToken);

    /// <summary>
    /// Получить список креативов с детальными данными
    /// </summary>
    Task<GetCreativesResponse> GetPageCreatives(PageRequest pageRequest, CancellationToken cancellationToken);

    /// <summary>
    /// Получить креатив по ERID
    /// </summary>
    Task<VkOrdApiCreativeV3Response> GetCreativeByErid(string erid, CancellationToken cancellationToken);
    
    /// <summary>
    /// Создать контрагента в VK ОРД из данных DaData по ИНН
    /// </summary>
    Task CreateCounterpartyFromInn(string inn, List<VkOrdApiPersonRoles> types, CancellationToken cancellationToken);

    /// <summary>
    /// Получить список всех контрагентов с полными данными из VK ОРД
    /// </summary>
    Task<GetCounterpartiesResponseDto> GetPageCounterparties(PageRequest pageRequest, CancellationToken cancellationToken);

    /// <summary>
    /// Получить контрагента по external_id из VK ОРД
    /// </summary>
    Task<VkOrdCounterparty> GetCounterpartyById(string externalId, CancellationToken cancellationToken);

    /// <summary>
    /// Загрузить медиа файл
    /// </summary>
    Task<string> UploadMedia(UploadMediaRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Получить информацию о медиа файле
    /// </summary>
    Task<VkOrdApiMediaInfoResponse> GetMedia(string externalId, CancellationToken cancellationToken);

    /// <summary>
    /// Получить список медиа файлов
    /// </summary>
    Task<VkOrdMediaInfoListResponseDto> GetPageMedia(PageRequest pageRequest, CancellationToken cancellationToken);
}
