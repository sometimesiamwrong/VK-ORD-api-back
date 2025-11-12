using Domain.Entities;
using Domain.Entities.Enums;
using Domain.Entities.Enums.VkOrd;
using Domain.Entities.VkOrd;
using Domain.Repositories.Interfaces.ApiCredentials;
using Domain.Repositories.Interfaces.VkOrd.Contract;
using Domain.Repositories.Interfaces.VkOrd.Counterparty;
using Domain.Repositories.Interfaces.VkOrd.Creative;
using Domain.Repositories.Interfaces.VkOrd.ErirStatus;
using Domain.Repositories.Interfaces.VkOrd.Invoice;
using Domain.Repositories.Interfaces.VkOrd.Statistics;
using Domain.Services.Interfaces;
using Domain.VkOrdApi;
using Domain.VkOrdApi.ErirStatus;
using Jobs.Configuration;
using Microsoft.Extensions.Options;

namespace Domain.Services.Implementations;

/// <summary>
/// Реализация сервиса синхронизации ERIR статусов
/// </summary>
public class ErirStatusSyncService : IErirStatusSyncService
{
    private readonly IVkOrdApiClientFactory _apiClientFactory;
    private readonly IGetAllLogicalAccountsRepository _logicalAccountsRepository;
    private readonly IVkOrdErirStatusRepository _erirStatusRepository;
    private readonly IGetCounterpartyByIdRepository _counterpartyRepository;
    private readonly IGetContractRepository _contractRepository;
    private readonly IGetCreativeRepository _creativeRepository;
    private readonly IGetInvoiceRepository _invoiceRepository;
    private readonly IGetStatisticsByIdRepository _statisticsRepository;
    private readonly ILogger<ErirStatusSyncService> _logger;
    private readonly JobsConfiguration _config;

    // Текущий credential для использования при загрузке сущностей
    private ApiCredential? _currentCredential;

    public ErirStatusSyncService(
        IVkOrdApiClientFactory apiClientFactory,
        IGetAllLogicalAccountsRepository logicalAccountsRepository,
        IVkOrdErirStatusRepository erirStatusRepository,
        IGetCounterpartyByIdRepository counterpartyRepository,
        IGetContractRepository contractRepository,
        IGetCreativeRepository creativeRepository,
        IGetInvoiceRepository invoiceRepository,
        IGetStatisticsByIdRepository statisticsRepository,
        ILogger<ErirStatusSyncService> logger,
        IOptions<JobsConfiguration> config)
    {
        _apiClientFactory = apiClientFactory;
        _logicalAccountsRepository = logicalAccountsRepository;
        _erirStatusRepository = erirStatusRepository;
        _counterpartyRepository = counterpartyRepository;
        _contractRepository = contractRepository;
        _creativeRepository = creativeRepository;
        _invoiceRepository = invoiceRepository;
        _statisticsRepository = statisticsRepository;
        _logger = logger;
        _config = config.Value;
    }

    public async Task SyncAllLogicalAccounts(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting ERIR status sync for all logical accounts");

        var logicalAccounts = await _logicalAccountsRepository.GetAllWithCredentials(cancellationToken);

        _logger.LogInformation("Found {Count} logical accounts to sync", logicalAccounts.Count);

        var successCount = 0;
        var errorCount = 0;

        foreach (var (logicalAccountId, credential) in logicalAccounts)
        {
            try
            {
                await SyncLogicalAccount(logicalAccountId, credential, cancellationToken);
                successCount++;
            }
            catch (Exception ex)
            {
                errorCount++;
                _logger.LogError(ex, "Error syncing logical account {LogicalAccountId}", logicalAccountId);

                if (!_config.ContinueOnError)
                {
                    throw;
                }
            }
        }

        _logger.LogInformation(
            "ERIR status sync completed. Success: {SuccessCount}, Errors: {ErrorCount}",
            successCount, errorCount);
    }

    public async Task SyncLogicalAccount(long logicalAccountId, ApiCredential credential, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting ERIR status sync for logical account {LogicalAccountId}", logicalAccountId);

        // Устанавливаем credential контекст для всех вызовов репозиториев в этом async потоке
        using var credentialContext = _apiClientFactory.SetCredentialContext(credential);

        var apiClient = await _apiClientFactory.CreateClient();

        // Шаг 1: Получаем ВСЕ ERIR статусы из API для данного logical account (с пагинацией)
        var allErirStatuses = await GetAllErirStatusesFromApi(apiClient, cancellationToken);

        _logger.LogInformation(
            "Fetched {Count} ERIR statuses from API for logical account {LogicalAccountId}",
            allErirStatuses.Count, logicalAccountId);

        // Шаг 2: Группируем статусы по типу сущности (пропускаем неподдерживаемые типы)
        var statusesByType = allErirStatuses
            .Select(s => new { Status = s, EntityType = MapErirDataTypeToEntityType(s.DataType) })
            .Where(x => x.EntityType.HasValue)
            .GroupBy(x => x.EntityType!.Value)
            .ToDictionary(g => g.Key, g => g.Select(x => x.Status).ToList());

        var totalProcessed = 0;
        var totalUpdated = 0;
        var totalCreated = 0;

        // Шаг 3: Обрабатываем каждый тип сущности
        var enabledEntityTypes = _config.GetEnabledEntityTypes();

        foreach (var entityType in enabledEntityTypes)
        {
            if (!statusesByType.TryGetValue(entityType, out var statuses) || statuses.Count == 0)
            {
                _logger.LogInformation(
                    "No ERIR statuses found for {EntityType} in logical account {LogicalAccountId}",
                    entityType, logicalAccountId);
                continue;
            }

            try
            {
                var (processed, created, updated) = await ProcessEntityTypeStatuses(
                    logicalAccountId,
                    entityType,
                    statuses,
                    cancellationToken);

                totalProcessed += processed;
                totalCreated += created;
                totalUpdated += updated;

                _logger.LogInformation(
                    "Processed {EntityType} for account {LogicalAccountId}. Total: {Processed}, Created: {Created}, Updated: {Updated}",
                    entityType, logicalAccountId, processed, created, updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error processing {EntityType} for logical account {LogicalAccountId}",
                    entityType, logicalAccountId);

                if (!_config.ContinueOnError)
                {
                    throw;
                }
            }
        }

        _logger.LogInformation(
            "Completed ERIR status sync for logical account {LogicalAccountId}. Total processed: {TotalProcessed}, Created: {TotalCreated}, Updated: {TotalUpdated}",
            logicalAccountId, totalProcessed, totalCreated, totalUpdated);
    }

    /// <summary>
    /// Получает все ERIR статусы из API с пагинацией
    /// </summary>
    private async Task<List<VkOrdApiErirStatusResponse>> GetAllErirStatusesFromApi(
        IVkOrdApiClient apiClient,
        CancellationToken cancellationToken)
    {
        var allStatuses = new List<VkOrdApiErirStatusResponse>();
        var offset = 0;
        const int limit = 60000;

        while (true)
        {
            var request = new VkOrdApiErirStatusesRequest
            {
                Offset = offset,
                Limit = limit
            };

            var response = await apiClient.GetErirStatuses(request, cancellationToken);

            if (response.Items.Count > 0)
            {
                allStatuses.AddRange(response.Items);
                _logger.LogDebug("Fetched {Count} ERIR statuses at offset {Offset}", response.Items.Count, offset);
            }

            // Если вернулось меньше лимита, значит это последняя страница
            if (response.Items.Count < limit)
            {
                break;
            }

            offset += limit;
        }

        return allStatuses;
    }

    /// <summary>
    /// Маппинг VkOrdApiErirDataType в EntityType
    /// Возвращает null для неподдерживаемых типов (Pad, Cid)
    /// </summary>
    private EntityType? MapErirDataTypeToEntityType(VkOrdApiErirDataType dataType)
    {
        return dataType switch
        {
            VkOrdApiErirDataType.Person => EntityType.Counterparty,
            VkOrdApiErirDataType.Contract => EntityType.Contract,
            VkOrdApiErirDataType.Creative => EntityType.Creative,
            VkOrdApiErirDataType.Invoice => EntityType.Invoice,
            VkOrdApiErirDataType.Statistics => EntityType.Statistic,
            VkOrdApiErirDataType.Pad => null, // Пока не поддерживается
            VkOrdApiErirDataType.Cid => null, // Пока не поддерживается
            _ => null // Неизвестные типы пропускаем
        };
    }

    /// <summary>
    /// Обрабатывает ERIR статусы для конкретного типа сущности
    /// </summary>
    private async Task<(int Processed, int Created, int Updated)> ProcessEntityTypeStatuses(
        long logicalAccountId,
        EntityType entityType,
        List<VkOrdApiErirStatusResponse> statuses,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Processing {Count} ERIR statuses for {EntityType} in logical account {LogicalAccountId}",
            statuses.Count, entityType, logicalAccountId);

        // Шаг 1: Получаем все existing external_ids из БД для данного типа и logical account
        var existingExternalIds = await _erirStatusRepository.GetAllExternalIdsFromVkOrdEntities(
            logicalAccountId,
            entityType,
            cancellationToken);

        var existingSet = new HashSet<string>(existingExternalIds);

        // Шаг 2: Разделяем статусы на "новые" и "существующие"
        var statusesToCreate = new List<VkOrdApiErirStatusResponse>();
        var statusesToUpdate = new List<VkOrdApiErirStatusResponse>();

        foreach (var status in statuses)
        {
            if (existingSet.Contains(status.ExternalId))
            {
                statusesToUpdate.Add(status);
            }
            else
            {
                statusesToCreate.Add(status);
            }
        }

        _logger.LogInformation(
            "{EntityType}: {TotalCount} total, {CreateCount} to create, {UpdateCount} to update",
            entityType, statuses.Count, statusesToCreate.Count, statusesToUpdate.Count);

        var created = 0;
        var updated = 0;

        // Шаг 3: Создаем новые сущности через Get репозитории
        if (statusesToCreate.Count > 0)
        {
            created = await CreateMissingEntities(
                logicalAccountId,
                entityType,
                statusesToCreate,
                cancellationToken);
        }

        // Шаг 4: Обновляем существующие сущности и ERIR статусы
        if (statusesToUpdate.Count > 0)
        {
            updated = await UpdateExistingEntities(
                logicalAccountId,
                entityType,
                statusesToUpdate,
                cancellationToken);
        }

        return (statuses.Count, created, updated);
    }

    /// <summary>
    /// Создает недостающие сущности через Get репозитории параллельно с ограничением concurrency
    /// </summary>
    private async Task<int> CreateMissingEntities(
        long logicalAccountId,
        EntityType entityType,
        List<VkOrdApiErirStatusResponse> statuses,
        CancellationToken cancellationToken)
    {
        if (entityType == EntityType.Statistic)
        {
            _logger.LogWarning(
                "Statistics entities should be created through CreateOrUpdateStatisticsRepository, skipping {Count} entities",
                statuses.Count);
            return 0;
        }

        var createdCount = 0;

        foreach (var status in statuses)
        {
            try
            {
                await LoadEntityFromApi(entityType, status.ExternalId, cancellationToken);
                await UpsertErirStatus(logicalAccountId, entityType, status, cancellationToken);
                createdCount++;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to create {EntityType} with external_id {ExternalId}",
                    entityType, status.ExternalId);

                if (!_config.ContinueOnError)
                {
                    throw;
                }
            }
        }

        _logger.LogInformation(
            "Created {Count} {EntityType} entities",
            createdCount, entityType);

        return createdCount;
    }

    /// <summary>
    /// Обновляет существующие сущности и их ERIR статусы
    /// </summary>
    private async Task<int> UpdateExistingEntities(
        long logicalAccountId,
        EntityType entityType,
        List<VkOrdApiErirStatusResponse> statuses,
        CancellationToken cancellationToken)
    {
        var updatedCount = 0;

        // Получаем все existing ERIR статусы для данного типа
        var existingStatuses = await _erirStatusRepository.GetAllByLogicalAccount(
            logicalAccountId,
            entityType,
            cancellationToken);

        var existingStatusMap = existingStatuses
            .ToDictionary(s => s.ExternalId, s => s);

        foreach (var apiStatus in statuses)
        {
            try
            {
                var needsUpdate = false;

                // Проверяем нужно ли обновить ERIR статус
                if (existingStatusMap.TryGetValue(apiStatus.ExternalId, out var existingStatus))
                {
                    var apiUpdatedTs = DateTimeOffset.Parse(apiStatus.UpdatedByUserTs);

                    if (existingStatus.UpdatedByUserTs < apiUpdatedTs)
                    {
                        needsUpdate = true;
                    }
                }
                else
                {
                    // Сущность есть, но статуса нет - создаем статус
                    needsUpdate = true;
                }

                if (needsUpdate)
                {
                    // Обновляем сущность из API (на случай если изменились данные)
                    await LoadEntityFromApi(entityType, apiStatus.ExternalId, cancellationToken);

                    // Обновляем ERIR статус
                    await UpsertErirStatus(logicalAccountId, entityType, apiStatus, cancellationToken);

                    updatedCount++;

                    _logger.LogDebug(
                        "Updated {EntityType} {ExternalId} with ERIR status {ErirStatus}",
                        entityType, apiStatus.ExternalId, apiStatus.ErirStatus);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to update {EntityType} with external_id {ExternalId}",
                    entityType, apiStatus.ExternalId);

                if (!_config.ContinueOnError)
                {
                    throw;
                }
            }
        }

        _logger.LogInformation(
            "Updated {Count} {EntityType} entities",
            updatedCount, entityType);

        return updatedCount;
    }

    /// <summary>
    /// Загружает сущность из API в БД через репозитории
    /// </summary>
    private async Task LoadEntityFromApi(
        EntityType entityType,
        string externalId,
        CancellationToken cancellationToken)
    {
        // Репозитории теперь работают с переданным credential через перегрузку CreateClient
        switch (entityType)
        {
            case EntityType.Counterparty:
                await _counterpartyRepository.Get(externalId, cancellationToken, noCache: true);
                break;

            case EntityType.Contract:
                await _contractRepository.Get(externalId, cancellationToken, noCache: true);
                break;

            case EntityType.Creative:
                await _creativeRepository.Get(externalId, cancellationToken, noCache: true);
                break;

            case EntityType.Invoice:
                await _invoiceRepository.Get(externalId, cancellationToken, noCache: true);
                break;

            case EntityType.Statistic:
                _logger.LogWarning("Statistics loading is not supported through this method");
                break;

            default:
                throw new ArgumentException($"Unsupported entity type: {entityType}", nameof(entityType));
        }
    }

    /// <summary>
    /// Создает или обновляет ERIR статус для сущности
    /// </summary>
    private async Task UpsertErirStatus(
        long logicalAccountId,
        EntityType entityType,
        VkOrdApiErirStatusResponse apiStatus,
        CancellationToken cancellationToken)
    {
        var status = new VkOrdErirStatus
        {
            LogicalAccountId = logicalAccountId,
            ExternalId = apiStatus.ExternalId,
            EntityType = entityType,
            ErirStatus = apiStatus.ErirStatus,
            UpdatedByUserTs = DateTimeOffset.Parse(apiStatus.UpdatedByUserTs),
            FinalizedTs = apiStatus.FinalizedTs != null ? DateTimeOffset.Parse(apiStatus.FinalizedTs) : null,
            ErrorMessages = apiStatus.Messages
        };

        await _erirStatusRepository.UpsertStatus(status, cancellationToken);
    }
}
