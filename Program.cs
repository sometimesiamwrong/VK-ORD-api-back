using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Polly;
using Polly.Extensions.Http;
using Refit;
using Serilog;
using System.Text;
using VkOrdApiWrapper.Configuration;
using VkOrdApiWrapper.Middleware;
using VkOrdApiWrapper.Services.Implementations;
using VkOrdApiWrapper.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using System.Net.Http.Headers;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.EntityFrameworkCore;
using VkOrdApiWrapper.Data;
using VkOrdApiWrapper.Security;
using VkOrdApiWrapper.Filters;

var builder = WebApplication.CreateBuilder(args);

// Настройка Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

// Настройка конфигурации
builder.Services.Configure<VkOrdConfiguration>(
    builder.Configuration.GetSection(VkOrdConfiguration.SectionName));
builder.Services.Configure<JwtConfiguration>(
    builder.Configuration.GetSection("JwtSettings"));
builder.Services.Configure<DaDataConfiguration>(
    builder.Configuration.GetSection(DaDataConfiguration.SectionName));
builder.Services.Configure<OpenRouterConfiguration>(
    builder.Configuration.GetSection(OpenRouterConfiguration.SectionName));
builder.Services.Configure<RedisConfiguration>(
    builder.Configuration.GetSection(RedisConfiguration.SectionName));

// Регистрация HTTP клиентов и Refit
var vkOrdConfig = builder.Configuration.GetSection(VkOrdConfiguration.SectionName).Get<VkOrdConfiguration>();
if (vkOrdConfig == null)
{
    throw new InvalidOperationException("VK ORD configuration is missing");
}

// DaData Refit client
var dadataConfig = builder.Configuration.GetSection(DaDataConfiguration.SectionName).Get<DaDataConfiguration>();
builder.Services.AddRefitClient<IDaDataApiClient>()
    .ConfigureHttpClient(client =>
    {
        client.BaseAddress = new Uri(dadataConfig?.BaseUrl ?? "https://suggestions.dadata.ru/suggestions/api/4_1/rs/");
        client.Timeout = TimeSpan.FromSeconds(dadataConfig?.TimeoutSeconds ?? 30);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        if (!string.IsNullOrWhiteSpace(dadataConfig?.ApiToken))
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Token {dadataConfig.ApiToken}");
        }
    });

// OpenRouter Refit client
var openRouterConfig = builder.Configuration.GetSection(OpenRouterConfiguration.SectionName).Get<OpenRouterConfiguration>();
builder.Services.AddRefitClient<IOpenRouterApiClient>()
    .ConfigureHttpClient(client =>
    {
        client.BaseAddress = new Uri("https://openrouter.ai");
        client.Timeout = TimeSpan.FromSeconds(openRouterConfig?.TimeoutSeconds ?? 30);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        var apiKey = Environment.GetEnvironmentVariable("OPENROUTER_API_KEY") ??
                    openRouterConfig?.ApiKey;
        if (!string.IsNullOrWhiteSpace(apiKey))
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
        }
        client.DefaultRequestHeaders.Add("HTTP-Referer", "https://vkord-api-wrapper.com");
        client.DefaultRequestHeaders.Add("X-Title", "VK ORD API Wrapper");
    });

// Регистрация сервисов
builder.Services.AddScoped<IVkOrdService, VkOrdService>();
builder.Services.AddScoped<IVkOrdApiClientFactory, VkOrdApiClientFactory>();
builder.Services.AddScoped<IDaDataService, DaDataService>();
builder.Services.AddScoped<IAiService, AiService>();
builder.Services.AddScoped<IDatabaseScriptService, DatabaseScriptService>();

// Регистрация фильтров
builder.Services.AddScoped<VkOrdApiWrapper.Controllers.Filters.VkApiHeadersFilter>();
builder.Services.AddScoped<EnsureAuthorizedRequestFilter>();

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
}
else
{
    builder.Services.AddDistributedMemoryCache();
}

// Настройка JWT аутентификации
var jwtConfig = builder.Configuration.GetSection("JwtSettings").Get<JwtConfiguration>();
if (jwtConfig == null)
{
    throw new InvalidOperationException("JWT configuration is missing");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtConfig.Issuer,
            ValidAudience = jwtConfig.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.SecretKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// DbContext (PostgreSQL)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? builder.Configuration["ConnectionStrings:DefaultConnection"];
if (string.IsNullOrWhiteSpace(connectionString))
{
    Log.Warning("Database connection string is missing. Set ConnectionStrings:DefaultConnection.");
}
else
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(connectionString));
}

// Security services
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<ISecretProtector, SecretProtector>();

// Добавление контроллеров
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.DateFormatString = "yyyy-MM-dd";
        options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
    })
    .AddMvcOptions(o =>
    {
        o.Filters.AddService<EnsureAuthorizedRequestFilter>();
    });

// Настройка Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "VK ОРД API Wrapper",
        Version = "v1",
        Description = "API для работы с VK ОРД (Оператор Рекламных Данных)"
    });

    // Настройка JWT в Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // Включение XML документации
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Настройка CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("https://criminally-astute-kangaroo.cloudpub.ru","https://ad-layer.ru", "https://server273.hosting.reg.ru", "http://ad-lawyer.ru", "http://localhost", "http://localhost:5173")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

var app = builder.Build();

// Выполнение SQL-скриптов при запуске приложения
using (var scope = app.Services.CreateScope())
{
    try
    {
        var scriptService = scope.ServiceProvider.GetRequiredService<IDatabaseScriptService>();
        var executedCount = await scriptService.ExecutePendingScriptsAsync();
        Log.Information("Database scripts execution completed. Executed {Count} scripts.", executedCount);
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Error executing database scripts during startup");
        // Не останавливаем приложение из-за ошибок в скриптах
    }
}

// Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "VK ОРД API Wrapper v1");
        c.RoutePrefix = string.Empty; // Swagger на корневом пути
    });
}

app.UseCors("AllowFrontend");
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
app.UseResultWrapper();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Добавляем health check endpoint
app.MapGet("/", () => new { Status = "Healthy", Timestamp = DateTime.UtcNow });

// Логируем адреса при запуске приложения
var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
lifetime.ApplicationStarted.Register(() =>
{
    var serverAddressesFeature = app.Services.GetRequiredService<IServer>().Features.Get<IServerAddressesFeature>();
    if (serverAddressesFeature != null)
    {
        Log.Information("Приложение запущено и слушает на следующих адресах:");
        foreach (var address in serverAddressesFeature.Addresses)
        {
            Log.Information("  {Address}", address);
        }
    }
    else
    {
        Log.Warning("Не удалось получить информацию об адресах сервера");
    }
});

app.Run();