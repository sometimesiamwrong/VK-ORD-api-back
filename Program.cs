using Microsoft.AspNetCore.Authentication.JwtBearer;
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
builder.Services.Configure<RedisConfiguration>(
    builder.Configuration.GetSection(RedisConfiguration.SectionName));

// Регистрация HTTP клиентов и Refit
var vkOrdConfig = builder.Configuration.GetSection(VkOrdConfiguration.SectionName).Get<VkOrdConfiguration>();
if (vkOrdConfig == null)
{
    throw new InvalidOperationException("VK ORD configuration is missing");
}
builder.Services.AddRefitClient<IVkOrdApiClient>()
    .ConfigureHttpClient(client =>
    {
        client.BaseAddress = new Uri(vkOrdConfig.GetApiUrl());
        client.Timeout = TimeSpan.FromSeconds(vkOrdConfig.TimeoutSeconds);
        client.DefaultRequestHeaders.Add("User-Agent", "VkOrdApiWrapper/1.0");
    });

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

// Регистрация сервисов
builder.Services.AddScoped<IVkOrdService, VkOrdService>();
builder.Services.AddScoped<IDaDataService, DaDataService>();

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

// Добавление контроллеров
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.DateFormatString = "yyyy-MM-dd";
        options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
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

// EF Core: SQLite local database
var connectionString = builder.Configuration.GetConnectionString("Default") ??
                       "Data Source=vkord.db";
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite(connectionString);
});

var app = builder.Build();

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

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
app.UseResultWrapper();

app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Добавляем health check endpoint
app.MapGet("/health", () => new { Status = "Healthy", Timestamp = DateTime.UtcNow });

// Ensure database created
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.Run();