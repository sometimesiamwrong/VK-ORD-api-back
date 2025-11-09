using Domain.Entities;
using Domain.Entities.Enums;
using Domain.Entities.VkOrd;
using Domain.Helpers;
using Domain.VkOrdApi.ErirStatus;
using Jobs.Configuration;
using Jobs.Services.Interfaces;
using Microsoft.Extensions.Options;
using WebApp.Repositories.Interfaces.ApiCredentials;
using WebApp.Repositories.Interfaces.VkOrd.Contract;
using WebApp.Repositories.Interfaces.VkOrd.Counterparty;
using WebApp.Repositories.Interfaces.VkOrd.Creative;
using WebApp.Repositories.Interfaces.VkOrd.ErirStatus;
using WebApp.Repositories.Interfaces.VkOrd.Invoice;
using WebApp.Repositories.Interfaces.VkOrd.Statistics;

namespace Jobs.Services.Implementations;

/// <summary>
/// Реализация сервиса синхронизации ERIR статусов
/// </summary>
public class ErirStatusSyncService : IErirStatusSyncService
{
    private readonly IBackgroundVkOrdApiClientFactory _apiClientFactory;
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
        IBackgroundVkOrdApiClientFactory apiClientFactory,
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

        var apiClient = await _apiClientFactory.CreateClient(credential);
        var enabledEntityTypes = _config.GetEnabledEntityTypes();

        var totalProcessed = 0;
        var totalUpdated = 0;

        foreach (var entityType in enabledEntityTypes)
        {
            try
            {
                var (processed, updated) = await SyncEntityType(
                    logicalAccountId,
                    entityType,
                    apiClient,
                    cancellationToken);

                totalProcessed += processed;
                totalUpdated += updated;

                _logger.LogInformation(
                    "Synced {EntityType} for account {LogicalAccountId}. Processed: {Processed}, Updated: {Updated}",
                    entityType, logicalAccountId, processed, updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error syncing {EntityType} for logical account {LogicalAccountId}",
                    entityType, logicalAccountId);

                if (!_config.ContinueOnError)
                {
                    throw;
                }
            }
        }

        _logger.LogInformation(
            "Completed ERIR status sync for logical account {LogicalAccountId}. Total processed: {TotalProcessed}, Total updated: {TotalUpdated}",
            logicalAccountId, totalProcessed, totalUpdated);
    }

    private async Task<(int Processed, int Updated)> SyncEntityType(
        long logicalAccountId,
        EntityType entityType,
        Domain.VkOrdApi.IVkOrdApiClient apiClient,
        CancellationToken cancellationToken)
    {
        var externalIds = await _erirStatusRepository.GetAllExternalIdsFromVkOrdEntities(
            logicalAccountId,
            entityType,
            cancellationToken);

        if (externalIds.Count == 0)
        {
            _logger.LogDebug("No entities found for {EntityType} in logical account {LogicalAccountId}",
                entityType, logicalAccountId);
            return (0, 0);
        }

        _logger.LogInformation("Found {Count} {EntityType} entities to check ERIR status",
            externalIds.Count, entityType);

        var processed = 0;
        var updated = 0;

        var dataType = EntityTypeMapper.ToErirDataType(entityType);
        var request = new VkOrdApiErirStatusesRequest
        {
            DataType = dataType,
            Limit = _config.BatchSize,
            ExternalIds = externalIds
        };

        var response = await apiClient.GetErirStatuses(request, cancellationToken);

        _logger.LogInformation("Received {Count} ERIR statuses from API for {EntityType}",
            response.Items.Count, entityType);

        foreach (var item in response.Items)
        {
            processed++;

            try
            {
                var wasUpdated = await ProcessErirStatusItem(
                    logicalAccountId,
                    entityType,
                    item,
                    cancellationToken);

                if (wasUpdated)
                {
                    updated++;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error processing ERIR status for {EntityType} with external_id {ExternalId}",
                    entityType, item.ExternalId);

                if (!_config.ContinueOnError)
                {
                    throw;
                }
            }
        }

        return (processed, updated);
    }

    private async Task<bool> ProcessErirStatusItem(
        long logicalAccountId,
        EntityType entityType,
        VkOrdApiErirStatusItem item,
        CancellationToken cancellationToken)
    {
        var existingStatus = await _erirStatusRepository.GetByExternalId(
            logicalAccountId,
            item.ExternalId,
            entityType,
            cancellationToken);

        var apiUpdatedTs = DateTimeOffset.Parse(item.UpdatedByUserTs);

        if (existingStatus != null && existingStatus.UpdatedByUserTs >= apiUpdatedTs)
        {
            return false;
        }

        _logger.LogInformation(
            "ERIR status changed for {EntityType} {ExternalId}: {ErirStatus}. Refreshing entity from API",
            entityType, item.ExternalId, item.ErirStatus);

        await RefreshEntityFromApi(entityType, item.ExternalId, cancellationToken);

        var newStatus = new VkOrdErirStatus
        {
            LogicalAccountId = logicalAccountId,
            ExternalId = item.ExternalId,
            EntityType = entityType,
            ErirStatus = item.ErirStatus,
            UpdatedByUserTs = apiUpdatedTs,
            FinalizedTs = item.FinalizedTs != null ? DateTimeOffset.Parse(item.FinalizedTs) : null,
            ErrorMessages = item.Messages
        };

        await _erirStatusRepository.UpsertStatus(newStatus, cancellationToken);

        return true;
    }

    private async Task RefreshEntityFromApi(EntityType entityType, string externalId, CancellationToken cancellationToken)
    {
        try
        {
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
                    await _statisticsRepository.Get(externalId, cancellationToken, noCache: true);
                    break;

                default:
                    _logger.LogWarning("Unsupported entity type for refresh: {EntityType}", entityType);
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to refresh {EntityType} with external_id {ExternalId} from API",
                entityType, externalId);
        }
    }
}
