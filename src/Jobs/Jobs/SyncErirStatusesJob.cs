using Hangfire;
using Jobs.Services.Interfaces;

namespace Jobs.Jobs;

/// <summary>
/// Фоновая задача для синхронизации ERIR статусов
/// </summary>
public class SyncErirStatusesJob
{
    private readonly IErirStatusSyncService _syncService;
    private readonly ILogger<SyncErirStatusesJob> _logger;

    public SyncErirStatusesJob(
        IErirStatusSyncService syncService,
        ILogger<SyncErirStatusesJob> logger)
    {
        _syncService = syncService;
        _logger = logger;
    }

    /// <summary>
    /// Выполнить синхронизацию ERIR статусов для всех логических аккаунтов
    /// </summary>
    [AutomaticRetry(Attempts = 3, DelaysInSeconds = new[] { 60, 300, 900 })]
    public async Task Execute()
    {
        _logger.LogInformation("Starting ERIR status sync job");

        try
        {
            await _syncService.SyncAllLogicalAccounts(CancellationToken.None);
            _logger.LogInformation("ERIR status sync job completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ERIR status sync job failed");
            throw;
        }
    }
}
