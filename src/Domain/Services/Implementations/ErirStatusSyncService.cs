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
        var syncStartTime = DateTime.UtcNow;
        _logger.LogInformation("Starting ERIR status sync for all logical accounts at {StartTime}", syncStartTime);

        var logicalAccounts = await _logicalAccountsRepository.GetAllWithCredentials(cancellationToken);

        _logger.LogInformation("Found {Count} logical accounts to sync", logicalAccounts.Count);

        // Шаг 1: Параллельно получаем ERIR статусы для всех аккаунтов (по 20 одновременно)
        var fetchStartTime = DateTime.UtcNow;
        var erirStatusesByAccount = await FetchAllErirStatusesInParallel(logicalAccounts, cancellationToken);
        var fetchDuration = DateTime.UtcNow - fetchStartTime;

        _logger.LogInformation(
            "Fetched ERIR statuses for {Count} logical accounts in {Duration:F2} seconds",
            erirStatusesByAccount.Count, fetchDuration.TotalSeconds);

        var successCount = 0;
        var errorCount = 0;

        // Шаг 2: Синхронизируем каждый аккаунт параллельно для ускорения
        // Используем ограниченный параллелизм для избежания перегрузки БД
        var maxConcurrency = _config.MaxConcurrency > 0 ? _config.MaxConcurrency : 5;
        var semaphore = new SemaphoreSlim(maxConcurrency, maxConcurrency);

        _logger.LogInformation(
            "Starting parallel sync for {Count} logical accounts with max concurrency: {MaxConcurrency}",
            logicalAccounts.Count, maxConcurrency);

        var processingStartTime = DateTime.UtcNow;
        var syncTasks = logicalAccounts.Select(async account =>
        {
            await semaphore.WaitAsync(cancellationToken);
            try
            {
                var accountStartTime = DateTime.UtcNow;
                await SyncLogicalAccount(account.LogicalAccountId, account.Credential, cancellationToken, erirStatusesByAccount);
                var accountDuration = DateTime.UtcNow - accountStartTime;

                _logger.LogDebug(
                    "Synced logical account {LogicalAccountId} in {Duration:F2} seconds",
                    account.LogicalAccountId, accountDuration.TotalSeconds);

                Interlocked.Increment(ref successCount);
            }
            catch (Exception ex)
            {
                Interlocked.Increment(ref errorCount);
                _logger.LogError(ex, "Error syncing logical account {LogicalAccountId}", account.LogicalAccountId);

                if (!_config.ContinueOnError)
                {
                    throw;
                }
            }
            finally
            {
                semaphore.Release();
            }
        });

        await Task.WhenAll(syncTasks);
        var processingDuration = DateTime.UtcNow - processingStartTime;

        var totalDuration = DateTime.UtcNow - syncStartTime;

        _logger.LogInformation(
            "ERIR status sync completed in {TotalDuration:F2} seconds. " +
            "Fetch phase: {FetchDuration:F2}s, Processing phase: {ProcessingDuration:F2}s. " +
            "Success: {SuccessCount}, Errors: {ErrorCount}. " +
            "Average per account: {AvgDuration:F2}s",
            totalDuration.TotalSeconds, fetchDuration.TotalSeconds, processingDuration.TotalSeconds,
            successCount, errorCount,
            successCount > 0 ? processingDuration.TotalSeconds / successCount : 0);
    }

    public async Task SyncLogicalAccount(
        long logicalAccountId,
        ApiCredential credential,
        CancellationToken cancellationToken,
        Dictionary<long, List<VkOrdApiErirStatusResponse>>? erirStatusesByAccount = null)
    {
        _logger.LogInformation("Starting ERIR status sync for logical account {LogicalAccountId}", logicalAccountId);

        // Устанавливаем credential контекст для всех вызовов репозиториев в этом async потоке
        using var credentialContext = _apiClientFactory.SetCredentialContext(credential);

        List<VkOrdApiErirStatusResponse> allErirStatuses;

        // Шаг 1: Получаем ERIR статусы - либо из переданного dictionary, либо из API
        if (erirStatusesByAccount != null && erirStatusesByAccount.TryGetValue(logicalAccountId, out var cachedStatuses))
        {
            allErirStatuses = cachedStatuses;
            _logger.LogInformation(
                "Using cached {Count} ERIR statuses for logical account {LogicalAccountId}",
                allErirStatuses.Count, logicalAccountId);
        }
        else
        {
            // Fallback: если не нашли в dictionary, получаем из API (обратная совместимость)
            _logger.LogInformation(
                "No cached statuses found for logical account {LogicalAccountId}, fetching from API",
                logicalAccountId);

            var apiClient = await _apiClientFactory.CreateClient();
            allErirStatuses = await GetAllErirStatusesFromApi(apiClient, cancellationToken);

            _logger.LogInformation(
                "Fetched {Count} ERIR statuses from API for logical account {LogicalAccountId}",
                allErirStatuses.Count, logicalAccountId);
        }

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
    /// Параллельно получает ERIR статусы для всех логических аккаунтов (по 20 одновременно)
    /// </summary>
    /// <param name="logicalAccounts">Список пар (LogicalAccountId, Credential)</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Dictionary с ERIR статусами для каждого аккаунта</returns>
    private async Task<Dictionary<long, List<VkOrdApiErirStatusResponse>>> FetchAllErirStatusesInParallel(
        List<(long LogicalAccountId, ApiCredential Credential)> logicalAccounts,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting parallel fetch of ERIR statuses for {Count} accounts", logicalAccounts.Count);

        var result = new Dictionary<long, List<VkOrdApiErirStatusResponse>>();
        var resultLock = new object();

        const int batchSize = 20;
        var batches = logicalAccounts
            .Select((account, index) => new { account, index })
            .GroupBy(x => x.index / batchSize)
            .Select(g => g.Select(x => x.account).ToList())
            .ToList();

        _logger.LogInformation("Processing {BatchCount} batches of up to {BatchSize} accounts", batches.Count, batchSize);

        foreach (var batch in batches)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            var batchTasks = batch.Select(async account =>
            {
                try
                {
                    _logger.LogDebug("Fetching ERIR statuses for logical account {LogicalAccountId}", account.LogicalAccountId);

                    // Устанавливаем credential контекст для данного аккаунта
                    using var credentialContext = _apiClientFactory.SetCredentialContext(account.Credential);
                    var apiClient = await _apiClientFactory.CreateClient();

                    // Получаем все статусы для данного аккаунта
                    var statuses = await GetAllErirStatusesFromApi(apiClient, cancellationToken);

                    lock (resultLock)
                    {
                        result[account.LogicalAccountId] = statuses;
                    }

                    _logger.LogDebug(
                        "Fetched {Count} ERIR statuses for logical account {LogicalAccountId}",
                        statuses.Count, account.LogicalAccountId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Error fetching ERIR statuses for logical account {LogicalAccountId}",
                        account.LogicalAccountId);

                    if (!_config.ContinueOnError)
                    {
                        throw;
                    }

                    // При ошибке добавляем пустой список, чтобы логика fallback сработала
                    lock (resultLock)
                    {
                        result[account.LogicalAccountId] = new List<VkOrdApiErirStatusResponse>();
                    }
                }
            });

            await Task.WhenAll(batchTasks);
        }

        _logger.LogInformation(
            "Parallel fetch completed. Successfully fetched for {SuccessCount}/{TotalCount} accounts",
            result.Count, logicalAccounts.Count);

        return result;
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

        try
        {
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
        }
        catch (Refit.ApiException ex ) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            //
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
    /// Создает недостающие сущности через Get репозитории
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

        // Обработать статусы пачками по 20 штук за раз
        var results = new List<bool>();
        const int batchSize = 20;
        for (int i = 0; i < statuses.Count; i += batchSize)
        {
            var batch = statuses.Skip(i).Take(batchSize).ToList();

            var batchTasks = batch.Select(async status =>
            {
                try
                {
                    await LoadEntityFromApi(entityType, status.ExternalId, cancellationToken);
                    await UpsertErirStatus(logicalAccountId, entityType, status, cancellationToken);
                    return true;
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

                    return false;
                }
            });

            var batchResults = await Task.WhenAll(batchTasks);
            results.AddRange(batchResults);

            // Если отменен токен, дальше не продолжаем
            if (cancellationToken.IsCancellationRequested)
                break;
        }

        createdCount = results.Count(success => success);

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

        // Списки для batch операций
        var statusesToUpdate = new List<VkOrdErirStatus>();
        var entitiesToRefresh = new List<string>();

        // Анализируем какие статусы и сущности нужно обновить
        foreach (var apiStatus in statuses)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            var needsStatusUpdate = false;
            var needsEntityRefresh = false;

            // Проверяем нужно ли обновить ERIR статус
            if (existingStatusMap.TryGetValue(apiStatus.ExternalId, out var existingStatus))
            {
                var apiUpdatedTs = DateTimeOffset.Parse(apiStatus.UpdatedByUserTs);

                // Проверяем изменился ли статус или timestamp
                if (existingStatus.UpdatedByUserTs < apiUpdatedTs || existingStatus.ErirStatus != apiStatus.ErirStatus)
                {
                    needsStatusUpdate = true;

                    // Обновляем сущность из API только если изменилась дата обновления
                    if (existingStatus.UpdatedByUserTs < apiUpdatedTs)
                    {
                        needsEntityRefresh = true;
                        _logger.LogDebug(
                            "Entity {EntityType} {ExternalId} needs refresh: ERIR UpdatedByUserTs changed from {Old} to {New}",
                            entityType, apiStatus.ExternalId, existingStatus.UpdatedByUserTs, apiUpdatedTs);
                    }
                    else
                    {
                        _logger.LogDebug(
                            "ERIR status changed for {EntityType} {ExternalId}: {Old} -> {New}, but timestamp unchanged",
                            entityType, apiStatus.ExternalId, existingStatus.ErirStatus, apiStatus.ErirStatus);
                    }
                }
            }
            else
            {
                // Сущность есть, но статуса нет - создаем статус и обновляем сущность
                needsStatusUpdate = true;
                needsEntityRefresh = true;
            }

            if (needsStatusUpdate)
            {
                // Добавляем в списки для batch обработки
                if (needsEntityRefresh)
                {
                    entitiesToRefresh.Add(apiStatus.ExternalId);
                }

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
                statusesToUpdate.Add(status);
                updatedCount++;
            }
        }

        // Обновляем сущности из API параллельно
        if (entitiesToRefresh.Count > 0)
        {
            const int refreshBatchSize = 10;
            var refreshTasks = new List<Task>();

            foreach (var batch in entitiesToRefresh.Select((x, i) => new { x, i })
                                                   .GroupBy(x => x.i / refreshBatchSize, x => x.x))
            {
                var batchTasks = batch.Select(externalId => Task.Run(async () =>
                {
                    try
                    {
                        await LoadEntityFromApi(entityType, externalId, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex,
                            "Failed to refresh {EntityType} with external_id {ExternalId}",
                            entityType, externalId);

                        if (!_config.ContinueOnError)
                        {
                            throw;
                        }
                    }
                }, cancellationToken));

                refreshTasks.AddRange(batchTasks);

                if (refreshTasks.Count >= refreshBatchSize)
                {
                    await Task.WhenAll(refreshTasks);
                    refreshTasks.Clear();
                }
            }

            if (refreshTasks.Count > 0)
            {
                await Task.WhenAll(refreshTasks);
            }

            _logger.LogInformation("Refreshed {Count} {EntityType} entities from API", entitiesToRefresh.Count, entityType);
        }

        // Обновляем все ERIR статусы одной batch операцией
        if (statusesToUpdate.Count > 0)
        {
            await _erirStatusRepository.UpsertStatusesBatch(statusesToUpdate, cancellationToken);
            _logger.LogInformation("Batch updated {Count} ERIR statuses for {EntityType}", statusesToUpdate.Count, entityType);
        }

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
