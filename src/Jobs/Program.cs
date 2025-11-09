using System.Text.Json;
using System.Text.Json.Serialization;
using Domain.Data;
using Hangfire;
using Hangfire.PostgreSql;
using Jobs.Configuration;
using Jobs.Jobs;
using Jobs.Services.Implementations;
using Jobs.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Serilog;
using WebApp.Configuration;
using WebApp.Repositories.Implementations.ApiCredentials;
using WebApp.Repositories.Implementations.VkOrd.Contract;
using WebApp.Repositories.Implementations.VkOrd.Counterparty;
using WebApp.Repositories.Implementations.VkOrd.Creative;
using WebApp.Repositories.Implementations.VkOrd.ErirStatus;
using WebApp.Repositories.Implementations.VkOrd.Invoice;
using WebApp.Repositories.Implementations.VkOrd.Statistics;
using WebApp.Repositories.Interfaces.ApiCredentials;
using WebApp.Repositories.Interfaces.VkOrd.Contract;
using WebApp.Repositories.Interfaces.VkOrd.Counterparty;
using WebApp.Repositories.Interfaces.VkOrd.Creative;
using WebApp.Repositories.Interfaces.VkOrd.ErirStatus;
using WebApp.Repositories.Interfaces.VkOrd.Invoice;
using WebApp.Repositories.Interfaces.VkOrd.Statistics;
using WebApp.Security;

var builder = WebApplication.CreateBuilder(args);

// Serilog configuration
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

// Configuration
builder.Services.Configure<JobsConfiguration>(builder.Configuration.GetSection(JobsConfiguration.SectionName));
builder.Services.Configure<VkOrdConfiguration>(builder.Configuration.GetSection(VkOrdConfiguration.SectionName));

// DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Hangfire with PostgreSQL
builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UsePostgreSqlStorage(c =>
        c.UseNpgsqlConnection(connectionString)));

builder.Services.AddHangfireServer(options =>
{
    options.WorkerCount = 1; // Single worker to avoid concurrency issues
    options.ServerName = "VkOrdErirSyncWorker";
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
builder.Services.AddScoped<IGetStatisticsByIdRepository, GetStatisticsByIdRepository>();
builder.Services.AddScoped<IGetApiCredentialByGuidRepository, GetApiCredentialByGuidRepository>();

// Services
builder.Services.AddScoped<IBackgroundVkOrdApiClientFactory, BackgroundVkOrdApiClientFactory>();
builder.Services.AddScoped<IErirStatusSyncService, ErirStatusSyncService>();

// Jobs
builder.Services.AddScoped<SyncErirStatusesJob>();

// HTTP Context Accessor (needed by some repository dependencies)
builder.Services.AddHttpContextAccessor();

// Health checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>();

var app = builder.Build();

// Hangfire Dashboard
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    DashboardTitle = "VK ORD ERIR Sync Jobs",
    StatsPollingInterval = 10000 // 10 seconds
});

// Health check endpoint
app.MapHealthChecks("/health");

// Configure recurring jobs
using (var scope = app.Services.CreateScope())
{
    var config = scope.ServiceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<JobsConfiguration>>().Value;

    RecurringJob.AddOrUpdate<SyncErirStatusesJob>(
        "sync-erir-statuses",
        job => job.Execute(),
        Cron.MinuteInterval(config.ErirSyncIntervalMinutes),
        new RecurringJobOptions
        {
            TimeZone = TimeZoneInfo.Utc
        });

    Log.Information("Configured ERIR sync job to run every {Interval} minutes", config.ErirSyncIntervalMinutes);
}

Log.Information("VK ORD ERIR Sync Jobs application started");
Log.Information("Hangfire Dashboard available at: /hangfire");

app.Run();
