using WebApp.Security;

namespace WebApp.Services;

public class DebugTokenCleanupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DebugTokenCleanupService> _logger;
    private readonly TimeSpan _cleanupInterval = TimeSpan.FromMinutes(10);

    public DebugTokenCleanupService(
        IServiceProvider serviceProvider,
        ILogger<DebugTokenCleanupService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Debug Token Cleanup Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(_cleanupInterval, stoppingToken);

                using var scope = _serviceProvider.CreateScope();
                var debugAccessService = scope.ServiceProvider.GetRequiredService<IDebugAccessService>();

                await debugAccessService.CleanupExpiredTokens();
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Debug Token Cleanup Service is stopping");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during debug token cleanup");
            }
        }

        _logger.LogInformation("Debug Token Cleanup Service stopped");
    }
}
