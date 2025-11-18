using System.Text.Json;
using System.Text.Json.Serialization;
using Domain.Data;
using Domain.Repositories.Implementations.ApiCredentials;
using Domain.Repositories.Implementations.VkOrd.Contract;
using Domain.Repositories.Implementations.VkOrd.Counterparty;
using Domain.Repositories.Implementations.VkOrd.Creative;
using Domain.Repositories.Implementations.VkOrd.ErirStatus;
using Domain.Repositories.Implementations.VkOrd.Invoice;
using Domain.Repositories.Implementations.VkOrd.Statistics;
using Domain.Repositories.Interfaces.ApiCredentials;
using Domain.Repositories.Interfaces.VkOrd.Contract;
using Domain.Repositories.Interfaces.VkOrd.Counterparty;
using Domain.Repositories.Interfaces.VkOrd.Creative;
using Domain.Repositories.Interfaces.VkOrd.ErirStatus;
using Domain.Repositories.Interfaces.VkOrd.Invoice;
using Domain.Repositories.Interfaces.VkOrd.Statistics;
using Domain.Services.Implementations;
using Domain.Services.Implementations.Cache;
using Domain.Services.Interfaces;
using Domain.VkOrdApi;
using Hangfire;
using Hangfire.PostgreSql;
using Jobs.Configuration;
using Jobs.Jobs;
using Microsoft.EntityFrameworkCore;
using Serilog;
using WebApp.Configuration;
using WebApp.Security;

var builder = WebApplication.CreateBuilder(args);

// Настройка конфигурации для использования jobs.appsettings.json
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("jobs.appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"jobs.appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// Serilog configuration
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

// Логирование текущего окружения
var environment = builder.Environment.EnvironmentName;
Log.Information("Starting Jobs application in {Environment} environment", environment);

// Configuration
builder.Services.Configure<JobsConfiguration>(builder.Configuration.GetSection(JobsConfiguration.SectionName));
builder.Services.Configure<VkOrdConfiguration>(builder.Configuration.GetSection(VkOrdConfiguration.SectionName));

// DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    Log.Error("Database connection string is missing. Set ConnectionStrings:DefaultConnection in appsettings.{Environment}.json", environment);
    throw new InvalidOperationException($"Database connection string is not configured for {environment} environment");
}

Log.Information("Configuring database connection for Jobs in {Environment} environment", environment);

// Регистрируем DbContextOptions для создания новых экземпляров
var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
    .UseNpgsql(connectionString)
    .Options;

if (builder.Environment.IsDevelopment())
{
    var devOptionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
        .UseNpgsql(connectionString)
        .EnableSensitiveDataLogging()
        .EnableDetailedErrors();
    dbContextOptions = devOptionsBuilder.Options;
}

// Регистрируем Func<AppDbContext> для создания нового подключения в рамках запроса
builder.Services.AddScoped<Func<AppDbContext>>(serviceProvider => () => new AppDbContext(dbContextOptions));

// Hangfire with PostgreSQL
builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UsePostgreSqlStorage(c =>
        c.UseNpgsqlConnection(connectionString), new PostgreSqlStorageOptions
        {
            DistributedLockTimeout = TimeSpan.FromMinutes(1), // Увеличиваем таймаут блокировки
            InvisibilityTimeout = TimeSpan.FromMinutes(30),
            QueuePollInterval = TimeSpan.FromSeconds(15),
            UseNativeDatabaseTransactions = true,
            PrepareSchemaIfNecessary = true,
            EnableTransactionScopeEnlistment = true
        }));

builder.Services.AddHangfireServer(options =>
{
    options.WorkerCount = 1; // Single worker to avoid concurrency issues
    options.ServerName = "VkOrdErirSyncWorker";
    options.ServerTimeout = TimeSpan.FromMinutes(5);
    options.ShutdownTimeout = TimeSpan.FromSeconds(30);
});

// JSON Serialization settings
builder.Services.AddSingleton(new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    Converters = { new JsonStringEnumConverter() }
});

// Security
builder.Services.AddSingleton<ISecretProtector, SecretProtector>();

// Repositories
builder.Services.AddScoped<IGetAllLogicalAccountsRepository, GetAllLogicalAccountsRepository>();
builder.Services.AddScoped<IVkOrdErirStatusRepository, VkOrdErirStatusRepository>();
builder.Services.AddScoped<IGetCounterpartyByIdRepository, GetCounterpartyByIdRepository>();
builder.Services.AddScoped<IGetContractRepository, GetContractRepository>();
builder.Services.AddScoped<IGetCreativeRepository, GetCreativeRepository>();
builder.Services.AddScoped<IGetInvoiceRepository, GetInvoiceRepository>();
builder.Services.AddScoped<ILogicalAccountMergeService, LogicalAccountMergeService>();
builder.Services.AddScoped<IGetStatisticsByIdRepository, GetStatisticsByIdRepository>();
builder.Services.AddScoped<IGetApiCredentialByGuidRepository, GetApiCredentialByGuidRepository>();

// Services
builder.Services.AddScoped<IBackgroundVkOrdApiClientFactory, BackgroundVkOrdApiClientFactory>();
builder.Services.AddScoped<IVkOrdApiClientFactory, VkOrdApiClientFactory>();
builder.Services.AddScoped<ICacheService, CacheService>();
// Настройка кэширования
builder.Services.AddMemoryCache();
var redisConfig = builder.Configuration.GetSection(RedisConfiguration.SectionName).Get<RedisConfiguration>();
if (redisConfig != null && !string.IsNullOrWhiteSpace(redisConfig.Configuration))
{
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = redisConfig.Configuration;
        options.InstanceName = redisConfig.InstanceName;
    });

    // Декорируем IDistributedCache для ограничения времени операций
    builder.Services.Decorate<Microsoft.Extensions.Caching.Distributed.IDistributedCache, TimeoutDistributedCache>();
}
else
{
    builder.Services.AddDistributedMemoryCache();
    // Даже для In-Memory кэша применим те же ограничения времени
    builder.Services.Decorate<Microsoft.Extensions.Caching.Distributed.IDistributedCache, TimeoutDistributedCache>();
}
builder.Services.AddScoped<IErirStatusSyncService, ErirStatusSyncService>();

// Jobs
builder.Services.AddScoped<SyncErirStatusesJob>();

// HTTP Context Accessor (needed by some repository dependencies)
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Hangfire Dashboard
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    DashboardTitle = "VK ORD ERIR Sync Jobs",
    StatsPollingInterval = 10000// 10 seconds
});


// Configure recurring jobs
using (var scope = app.Services.CreateScope())
{
    var config = scope.ServiceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<JobsConfiguration>>().Value;

    // Retry logic for registering recurring jobs (in case of lock timeout)
    int maxRetries = 3;
    int retryDelay = 5000; // 5 seconds
    
    for (int attempt = 1; attempt <= maxRetries; attempt++)
    {
        try
        {
            Log.Information("Attempting to register recurring jobs (attempt {Attempt}/{MaxRetries})", attempt, maxRetries);
            
            // Используем правильное Cron выражение вместо устаревшего Cron.MinuteInterval
            var cronExpression = config.ErirSyncIntervalMinutes == 1
                ? Cron.Minutely()
                : $"*/{config.ErirSyncIntervalMinutes} * * * *";

            RecurringJob.AddOrUpdate<SyncErirStatusesJob>(
                "sync-erir-statuses",
                job => job.Execute(),
                cronExpression,
                new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.Utc
                });

            Log.Information("Successfully configured ERIR sync job to run every {Interval} minutes (cron: {Cron}, timezone: UTC)",
                config.ErirSyncIntervalMinutes, cronExpression);

            // Триггерим немедленное выполнение для проверки
            RecurringJob.Trigger("sync-erir-statuses");
            Log.Information("Triggered immediate execution of ERIR sync job for testing");
            break; // Success - exit retry loop
        }
        catch (Hangfire.PostgreSql.PostgreSqlDistributedLockException ex)
        {
            if (attempt < maxRetries)
            {
                Log.Warning(ex, "Failed to acquire distributed lock for recurring job registration (attempt {Attempt}/{MaxRetries}). Retrying in {Delay}ms...", 
                    attempt, maxRetries, retryDelay);
                Thread.Sleep(retryDelay);
            }
            else
            {
                Log.Error(ex, "Failed to register recurring jobs after {MaxRetries} attempts. " +
                    "This may indicate that another instance is running or there's a stale lock. " +
                    "The application will continue, but recurring jobs may not be registered.", maxRetries);
                // Don't throw - allow the application to start anyway
                // The Hangfire dashboard will still be available for manual job management
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Unexpected error while registering recurring jobs");
            throw;
        }
    }
}

Log.Information("VK ORD ERIR Sync Jobs application started");
Log.Information("Hangfire Dashboard available at: /hangfire");

app.Run();
