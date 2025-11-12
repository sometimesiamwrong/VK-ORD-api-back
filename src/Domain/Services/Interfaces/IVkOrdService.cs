using Domain.Entities.Enums.VkOrd;
using Domain.Entities.VkOrd;
using Domain.Models.Contracts;
using Domain.Models.Requests;
using Domain.Models.Responses;
using Domain.VkOrdApi.Media;

namespace Domain.Services.Interfaces;

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
    Task<VkOrdContract> GetContract(string externalId, CancellationToken cancellationToken, bool noCache = false);

    /// <summary>
    /// Получить полные данные контракта (контракт + стороны + креативы + доп. соглашения)
    /// </summary>
    Task<GetContractDetailsDto?> GetContractDetails(string externalId, CancellationToken cancellationToken);

    /// <summary>
    /// Получить список контрактов
    /// </summary>
    Task<GetContractsDto> GetPageContract(PageRequest pageRequest, CancellationToken cancellationToken);

    /// <summary>
    /// Создать креатив
    /// </summary>
    Task<VkOrdCreative> CreateCreative(CreateCreativeRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Получить информацию о креативе по external_id
    /// </summary>
    Task<VkOrdCreative> GetCreative(string externalId, CancellationToken cancellationToken, bool noCache = false);

    /// <summary>
    /// Получить список креативов с детальными данными
    /// </summary>
    Task<GetCreativesResponse> GetPageCreatives(PageRequest pageRequest, CancellationToken cancellationToken);

    /// <summary>
    /// Получить креатив по ERID
    /// </summary>
    Task<VkOrdCreative> GetCreativeByErid(string erid, CancellationToken cancellationToken);
    
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

    /// <summary>
    /// Получить список договоров для контрагента
    /// </summary>
    Task<CounterpartyWithContractsDto> GetContractsToCounterparty(string externalId, CancellationToken cancellationToken);

    /// <summary>
    /// Создать или обновить акт
    /// </summary>
    Task CreateOrUpdateInvoice(CreateOrUpdateInvoiceRequest request, bool isDraft, CancellationToken cancellationToken);

    /// <summary>
    /// Получить информацию об акте по external_id
    /// </summary>
    Task<VkOrdInvoice> GetInvoice(string externalId, CancellationToken cancellationToken);

    /// <summary>
    /// Получить список актов с пагинацией
    /// </summary>
    Task<GetInvoicesDto> GetPageInvoice(PageRequest pageRequest, CancellationToken cancellationToken, List<string>? counterpartyExternalIds = null);

    /// <summary>
    /// Удалить акт
    /// </summary>
    Task DeleteInvoice(string externalId, CancellationToken cancellationToken);

    /// <summary>
    /// Добавить договоры в акт
    /// </summary>
    Task AddContractsToInvoice(string externalId, Domain.VkOrdApi.Invoice.VkOrdApiAddContractsToInvoiceRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Удалить договоры из акта
    /// </summary>
    Task DeleteContractsFromInvoice(string externalId, Domain.VkOrdApi.Invoice.VkOrdApiDeleteContractsFromInvoiceRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Отправить акт в ЕРИР
    /// </summary>
    Task SendInvoiceToErir(string externalId, CancellationToken cancellationToken);

    /// <summary>
    /// Создать или обновить заголовок акта (без разаллокаций)
    /// </summary>
    Task CreateOrUpdateInvoiceHeader(string externalId, Domain.VkOrdApi.Invoice.VkOrdApiInvoiceHeaderRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Получить заголовок акта (без разаллокаций)
    /// </summary>
    Task<Domain.VkOrdApi.Invoice.VkOrdApiInvoiceHeaderResponse> GetInvoiceHeader(string externalId, CancellationToken cancellationToken);

    // ============= Statistics =============

    /// <summary>
    /// Создать или обновить статистику
    /// </summary>
    Task CreateOrUpdateStatistics(List<Domain.VkOrdApi.Statistics.VkOrdApiStatisticsItem> items, CancellationToken cancellationToken);

    /// <summary>
    /// Получить список статистик с фильтрацией и пагинацией
    /// </summary>
    Task<GetStatisticsDto> GetStatisticsList(string? creativeExternalId, string? padExternalId, int offset, int limit, CancellationToken cancellationToken);

    /// <summary>
    /// Удалить статистику
    /// </summary>
    Task DeleteStatistics(List<DeleteStatisticsItemRequest> items, CancellationToken cancellationToken);
}
