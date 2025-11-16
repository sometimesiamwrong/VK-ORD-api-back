using Domain.Services.Interfaces;
using Hangfire;
using Jobs.Configuration;
using Microsoft.Extensions.Options;

namespace Jobs.Jobs;

/// <summary>
/// Фоновая задача для синхронизации ERIR статусов
/// </summary>
public class SyncErirStatusesJob
{
    private readonly IErirStatusSyncService _syncService;
    private readonly ILogger<SyncErirStatusesJob> _logger;
    private readonly JobsConfiguration _config;

    public SyncErirStatusesJob(
        IErirStatusSyncService syncService,
        ILogger<SyncErirStatusesJob> logger,
        IOptions<JobsConfiguration> config)
    {
        _syncService = syncService;
        _logger = logger;
        _config = config.Value;
    }

    /// <summary>
    /// Выполнить синхронизацию ERIR статусов для всех логических аккаунтов
    /// </summary>
    [AutomaticRetry(Attempts = 3, DelaysInSeconds = new[] { 60, 300, 900 })]
    public async Task Execute()
    {
        var startTime = DateTime.UtcNow;
        _logger.LogInformation("Starting ERIR status sync job at {StartTime:yyyy-MM-dd HH:mm:ss} UTC", startTime);

        try
        {
            await _syncService.SyncAllLogicalAccounts(CancellationToken.None);

            var endTime = DateTime.UtcNow;
            var duration = endTime - startTime;
            var nextRunTime = endTime.AddMinutes(_config.ErirSyncIntervalMinutes);

            _logger.LogInformation(
                "ERIR status sync job completed successfully in {Duration}. Next run scheduled at {NextRunTime:yyyy-MM-dd HH:mm:ss} UTC (in {IntervalMinutes} minutes)",
                duration.ToString(@"hh\:mm\:ss"),
                nextRunTime,
                _config.ErirSyncIntervalMinutes);

            Console.WriteLine($"✓ Job completed in {duration:hh\\:mm\\:ss}. Next run: {nextRunTime:yyyy-MM-dd HH:mm:ss} UTC");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ERIR status sync job failed");
            throw;
        }
    }
}
