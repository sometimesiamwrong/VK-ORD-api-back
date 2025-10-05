using Refit;
using Domain;
using VkOrdApi.Contract;
using VkOrdApi.Person;
using VkOrdApi.ErirStatus;
using VkOrdApi.Statistics;
using VkOrdApi.Invoice;
using VkOrdApi.Media;
using VkOrdApi.Creative;
using VkOrdApi.Pad;

/// <summary>
/// Методы клиента для работы с VK ОРД
/// </summary>
public partial interface IVkOrdApiClient
{  
    // Person methods
    /// <summary>
    /// Получить список всех контрагентов (persons) - возвращает external_ids с пагинацией
    /// </summary>
    [Get("/v1/person")]
    Task<VkOrdPersonListResponse> GetPersons([Query] PageRequest pageRequest, CancellationToken cancellationToken);

    /// <summary>
    /// Создать/обновить контрагента (person)
    /// </summary>
    [Put("/v1/person/{externalId}")]
    Task CreateOrUpdatePerson(
        string externalId,
        [Body] VkOrdPersonResponse personResponse,
        CancellationToken cancellationToken);

    /// <summary>
    /// Получить контрагента (person) по external_id
    /// </summary>
    [Get("/v1/person/{externalId}")]
    Task<VkOrdPersonResponse> GetPerson(string externalId, CancellationToken cancellationToken);

    // Contract methods
    /// <summary>
    /// Получить список всех договоров (contracts) - возвращает external_ids с пагинацией
    /// </summary>
    [Get("/v1/contract")]
    Task<VkOrdContractListResponse> GetContracts([Query] PageRequest pageRequest, CancellationToken cancellationToken);

    /// <summary>
    /// Создать/обновить договор (contract)
    /// </summary>
    [Put("/v1/contract/{external_id}")]
    Task CreateOrUpdateContract(
        [AliasAs("external_id")] string externalId,
        [Body] VkOrdCreateUpdateContractRequest contractRequest,
        CancellationToken cancellationToken,
        [Query] bool? updateAdditionalContractsParties = null);

    /// <summary>
    /// Получить договор (contract) по external_id
    /// </summary>
    [Get("/v1/contract/{external_id}")]
    Task<VkOrdContractResponse> GetContractByExternalId([AliasAs("external_id")] string externalId, CancellationToken cancellationToken);

    /// <summary>
    /// Запросить CID для контракта
    /// </summary>
    [Post("/v1/contract/{external_id}/create_cid")]
    Task RequestContractCid([AliasAs("external_id")] string externalId, CancellationToken cancellationToken);

    // ErirStatus methods
    /// <summary>
    /// Получить статус обработки конкретного объекта рекламы в ЕРИР (GET /v1/{data_type}/{external_id}/erir_status)
    /// data_type: тип объекта (e.g., 'invoice', 'creative'), external_id: уникальный ID объекта.
    /// Возвращает erir_status (processing/bad/verified), updated_by_user_ts, finalized_ts (if bad/verified), messages (if bad).
    /// </summary>
    [Get("/v1/{data_type}/{external_id}/erir_status")]
    Task<VkOrdErirStatusResponse> GetErirStatus(
        [AliasAs("data_type")] string dataType,
        [AliasAs("external_id")] string externalId);

    /// <summary>
    /// Получить список статусов обработки объектов рекламы в ЕРИР (GET /v1/erir_statuses)
    /// Фильтрация через request-объект: data_type (required enum: person/contract/creative/pad/invoice/statistics/cid),
    /// erir_status (optional enum: processing/bad/verified), offset (default 0), limit (default 10000, max 60000),
    /// limit_per_entity (optional, interacts with limit), external_ids (optional array of strings).
    /// Пагинированный список с erir_status, timestamps и messages где применимо.
    /// Порядок объектов: person, contract, creative, invoice, pad, statistics.
    /// </summary>
    [Get("/v1/erir_statuses")]
    Task<VkOrdErirStatusesListResponse> GetErirStatuses([Query] VkOrdErirStatusesRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Получить статусы обработки нескольких объектов рекламы в ЕРИР (POST /v1/erir_statuses)
    /// Body: запрос с фильтрами (data_type required, erir_status optional, offset=0, limit=10000, limit_per_entity optional, external_ids optional).
    /// Возвращает пагинированный список статусов. Аналогично GET, но для batch-запросов.
    /// </summary>
    [Post("/v1/erir_statuses")]
    Task<VkOrdErirStatusesListResponse> GetErirStatusesBatch([Body] VkOrdErirStatusesRequest request, CancellationToken cancellationToken);

    // Statistics methods
    /// <summary>
    /// Получить список статистик (v2, filters: creative_external_id?, pad_external_id?, date_start_actual_from?, date_start_actual_to?, offset?, limit?)
    /// </summary>
    [Get("/v2/statistics/list")]
    Task<VkOrdStatisticsListResponse> GetStatisticsListV2([Query] VkOrdStatisticsListRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Создать статистику (batch POST v2, body array items)
    /// </summary>
    [Post("/v2/statistics")]
    Task CreateStatisticsV2([Body(BodySerializationMethod.Serialized)] List<VkOrdStatisticsItem> items, CancellationToken cancellationToken);

    /// <summary>
    /// Удалить статистику (POST v1, body external_ids list)
    /// </summary>
    [Post("/v1/statistics/delete")]
    Task DeleteStatisticsV1([Body] VkOrdDeleteStatisticsRequest deleteRequest, CancellationToken cancellationToken);

    // Invoice methods
    /// <summary>
    /// Получить список всех актов - возвращает external_ids с пагинацией (v1)
    /// </summary>
    [Get("/v1/invoice")]
    Task<VkOrdInvoiceListResponse> GetInvoicesV1([Query] PageRequest pageRequest, CancellationToken cancellationToken);

    /// <summary>
    /// Удалить договоры из акта (v2)
    /// </summary>
    [Post("/v2/invoice/{external_id}/delete")]
    Task DeleteContractsFromInvoiceV2(
        [AliasAs("external_id")] string externalId,
        [Body] VkOrdDeleteContractsFromInvoiceRequest deleteRequest,
        CancellationToken cancellationToken);

    /// <summary>
    /// Отправить акт в ЕРИР (v2)
    /// </summary>
    [Post("/v2/invoice/{external_id}/ready")]
    Task SendInvoiceToErirV2(
        [AliasAs("external_id")] string externalId,
        [Body] VkOrdSendInvoiceToErirRequest sendRequest,
        CancellationToken cancellationToken);

    /// <summary>
    /// Создать полный акт (v3, detailed schema)
    /// </summary>
    [Put("/v3/invoice/{external_id}")]
    Task CreateFullInvoiceV3(
        [AliasAs("external_id")] string externalId,
        [Body] VkOrdFullInvoiceRequest fullRequest,
        CancellationToken cancellationToken);

    /// <summary>
    /// Получить полный акт (v3, detailed schema)
    /// </summary>
    [Get("/v3/invoice/{external_id}")]
    Task<VkOrdFullInvoiceResponse> GetFullInvoiceV3([AliasAs("external_id")] string externalId, CancellationToken cancellationToken);

    /// <summary>
    /// Удалить акт (v3)
    /// </summary>
    [Delete("/v3/invoice/{external_id}")]
    Task DeleteInvoiceV3([AliasAs("external_id")] string externalId, CancellationToken cancellationToken);

    /// <summary>
    /// Создать заголовок акта (v3)
    /// </summary>
    [Put("/v3/invoice/{external_id}/header")]
    Task CreateInvoiceHeaderV3(
        [AliasAs("external_id")] string externalId,
        [Body] VkOrdInvoiceHeaderRequest headerRequest,
        CancellationToken cancellationToken);

    /// <summary>
    /// Получить заголовок акта без разаллокаций (v3)
    /// </summary>
    [Get("/v3/invoice/{external_id}/header")]
    Task<VkOrdInvoiceHeaderResponse> GetInvoiceHeaderV3([AliasAs("external_id")] string externalId, CancellationToken cancellationToken);

    /// <summary>
    /// Добавить договоры в акт (v3)
    /// </summary>
    [Patch("/v3/invoice/{external_id}/items")]
    Task AddContractsToInvoiceV3(
        [AliasAs("external_id")] string externalId,
        [Body] VkOrdAddContractsToInvoiceRequest addRequest,
        CancellationToken cancellationToken);

    // Media methods
    /// <summary>
    /// Получить список всех медиафайлов - возвращает external_ids с пагинацией
    /// </summary>
    [Get("/v1/media")]
    Task<VkOrdMediaListResponse> GetPageMedia([Query] PageRequest pageRequest, CancellationToken cancellationToken);

    /// <summary>
    /// Загрузить медиафайл (multipart/form-data, поле 'file' для binary)
    /// </summary>
    [Multipart]
    [Put("/v1/media/{external_id}")]
    Task UploadMedia(
        [AliasAs("external_id")] string externalId,
        [AliasAs("file")] StreamPart file,
        CancellationToken cancellationToken);

    /// <summary>
    /// Получить бинарный медиафайл
    /// </summary>
    [Get("/v1/media/{external_id}")]
    Task<byte[]> GetMediaFile([AliasAs("external_id")] string externalId, CancellationToken cancellationToken);

    /// <summary>
    /// Получить данные медиафайла (info)
    /// </summary>
    [Get("/v1/media/{external_id}/info")]
    Task<VkOrdMediaInfoResponse> GetMediaInfo([AliasAs("external_id")] string externalId, CancellationToken cancellationToken);

    // Creative methods
    /// <summary>
    /// Получить список всех креативов - возвращает external_ids с пагинацией (v1)
    /// </summary>
    [Get("/v1/creative")]
    Task<VkOrdCreativeListResponse> GetCreativesV1([Query] PageRequest pageRequest, CancellationToken cancellationToken);

    /// <summary>
    /// Получить список маркеров рекламы (ERIDs) (v1)
    /// </summary>
    [Get("/v1/creative/list/erids")]
    Task<VkOrdEridsListResponse> GetEridsListV1([Query] PageRequest pageRequest, CancellationToken cancellationToken);

    /// <summary>
    /// Получить список пар маркеров рекламы и внешних идентификаторов (v1)
    /// </summary>
    [Get("/v1/creative/list/erid_external_ids")]
    Task<VkOrdEridExternalIdsListResponse> GetEridExternalIdsListV1([Query] PageRequest pageRequest, CancellationToken cancellationToken);

    /// <summary>
    /// Создать/обновить креатив (v2)
    /// </summary>
    [Put("/v2/creative/{external_id}")]
    Task CreateOrUpdateCreativeV2(
        [AliasAs("external_id")] string externalId,
        [Body] VkOrdCreativeV2Request creativeRequest,
        CancellationToken cancellationToken);

    /// <summary>
    /// Получить креатив (v2)
    /// </summary>
    [Get("/v2/creative/{external_id}")]
    Task<VkOrdCreativeV2Response> GetCreativeV2ByExternalId([AliasAs("external_id")] string externalId, CancellationToken cancellationToken);

    /// <summary>
    /// Получить креатив по маркеру рекламы (ERID) (v2)
    /// </summary>
    [Get("/v2/creative/by_erid/{erid}")]
    Task<VkOrdCreativeV2Response> GetCreativeV2ByErid([AliasAs("erid")] string erid, CancellationToken cancellationToken);

    /// <summary>
    /// Создать/обновить креатив (v3)
    /// </summary>
    [Put("/v3/creative/{external_id}")]
    Task<VkOrdCreativeV3RequestResponse> CreateOrUpdateCreativeV3(
        [AliasAs("external_id")] string externalId,
        [Body] VkOrdCreativeV3Request creativeRequest,
        CancellationToken cancellationToken);

    /// <summary>
    /// Получить креатив (v3)
    /// </summary>
    [Get("/v3/creative/{external_id}")]
    Task<VkOrdCreativeV3Response> GetCreativeV3ByExternalId([AliasAs("external_id")] string externalId, CancellationToken cancellationToken);

    /// <summary>
    /// Получить креатив по маркеру рекламы (ERID) (v3)
    /// </summary>
    [Get("/v3/creative/by_erid/{erid}")]
    Task<VkOrdCreativeV3Response> GetCreativeV3ByErid([AliasAs("erid")] string erid, CancellationToken cancellationToken);

    /// <summary>
    /// Добавить тексты в креатив (v1)
    /// </summary>
    [Post("/v1/creative/{external_id}/add_text")]
    Task AddTextToCreativeV1(
        [AliasAs("external_id")] string externalId,
        [Body] VkOrdAddTextRequest addTextRequest,
        CancellationToken cancellationToken);

    /// <summary>
    /// Добавить внешние ресурсы в креатив (v1)
    /// </summary>
    [Post("/v1/creative/{external_id}/add_external_media")]
    Task AddExternalMediaToCreativeV1(
        [AliasAs("external_id")] string externalId,
        [Body] VkOrdAddExternalMediaRequest addExternalMediaRequest,
        CancellationToken cancellationToken);

    /// <summary>
    /// Добавить медиафайлы в креатив (v1)
    /// </summary>
    [Post("/v1/creative/{external_id}/add_media")]
    Task AddMediaToCreativeV1(
        [AliasAs("external_id")] string externalId,
        [Body] VkOrdAddMediaRequest addMediaRequest,
        CancellationToken cancellationToken);

    // Pad methods
    /// <summary>
    /// Получить список всех платформ (pads) - возвращает external_ids с пагинацией
    /// </summary>
    [Get("/v1/pad")]
    Task<VkOrdPadListResponse> GetPads([Query] PageRequest pageRequest, CancellationToken cancellationToken);

    /// <summary>
    /// Создать/обновить платформу (pad)
    /// </summary>
    [Put("/v1/pad/{external_id}")]
    Task CreateOrUpdatePad(
        [AliasAs("external_id")] string externalId,
        [Body] VkOrdCreateUpdatePadRequest padRequest,
        CancellationToken cancellationToken);

    /// <summary>
    /// Получить платформу (pad) по external_id
    /// </summary>
    [Get("/v1/pad/{external_id}")]
    Task<VkOrdPadResponse> GetPadByExternalId([AliasAs("external_id")] string externalId, CancellationToken cancellationToken);

    /// <summary>
    /// Получить ограниченную информацию о платформах
    /// </summary>
    [Get("/v1/pad/info/restricted")]
    Task<VkOrdPadRestrictedInfoResponse> GetPadRestrictedInfo(CancellationToken cancellationToken);
}
