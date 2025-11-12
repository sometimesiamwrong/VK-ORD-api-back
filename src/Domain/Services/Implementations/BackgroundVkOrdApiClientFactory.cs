using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Domain.Entities;
using Domain.Services.Interfaces;
using Domain.VkOrdApi;
using Refit;
using WebApp.Security;

namespace Domain.Services.Implementations;

/// <summary>
/// Реализация фабрики для создания VK ORD API клиента в фоновых задачах
/// </summary>
public class BackgroundVkOrdApiClientFactory : IBackgroundVkOrdApiClientFactory
{
    private readonly ILogger<BackgroundVkOrdApiClientFactory> _logger;
    private readonly ILoggerFactory _loggerFactory;
    private readonly ISecretProtector _protector;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public BackgroundVkOrdApiClientFactory(
        ILogger<BackgroundVkOrdApiClientFactory> logger,
        ILoggerFactory loggerFactory,
        ISecretProtector protector,
        JsonSerializerOptions jsonSerializerOptions)
    {
        _logger = logger;
        _loggerFactory = loggerFactory;
        _protector = protector;

        _jsonSerializerOptions = new JsonSerializerOptions(jsonSerializerOptions);
        _jsonSerializerOptions.Converters.Insert(0, new EnumMemberJsonConverter());
    }

    public async Task<IVkOrdApiClient> CreateClient(ApiCredential credential)
    {
        var apiContext = GetApiContext(credential);
        var baseUrl = apiContext.GetBaseUrl();

        var httpClientHandler = new HttpClientHandler
        {
            MaxConnectionsPerServer = 10,
            UseCookies = false,
            AutomaticDecompression = System.Net.DecompressionMethods.GZip |
                                   System.Net.DecompressionMethods.Deflate |
                                   System.Net.DecompressionMethods.Brotli,
            UseProxy = false
        };

        var headerHandler = new BackgroundVkOrdApiHeaderHandler(_loggerFactory.CreateLogger<BackgroundVkOrdApiHeaderHandler>());
        var errorHandler = new BackgroundVkOrdApiErrorHandler(_loggerFactory.CreateLogger<BackgroundVkOrdApiErrorHandler>());

        headerHandler.InnerHandler = httpClientHandler;
        errorHandler.InnerHandler = headerHandler;

        var httpClient = new HttpClient(errorHandler)
        {
            BaseAddress = new Uri(baseUrl),
            Timeout = TimeSpan.FromSeconds(60),
            MaxResponseContentBufferSize = 10 * 1024 * 1024
        };

        httpClient.DefaultRequestHeaders.Add("Authorization", apiContext.GetAuthorizationHeader());
        httpClient.DefaultRequestHeaders.Add("User-Agent", "VK-ORD-API-Wrapper/1.0 (BackgroundJobs)");
        httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        httpClient.DefaultRequestHeaders.Add("Accept-Language", "ru-RU,ru;q=0.9,en;q=0.8");
        httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
        httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
        httpClient.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
        httpClient.DefaultRequestHeaders.Add("Pragma", "no-cache");
        httpClient.DefaultRequestHeaders.Add("X-Client-Version", "1.0.0");
        httpClient.DefaultRequestHeaders.Add("X-Client-Type", "BackgroundJobs");
        httpClient.DefaultRequestHeaders.Add("X-API-Route", apiContext.Route.ToString());

        _logger.LogDebug("Created VK ORD API client for LogicalAccountId: {LogicalAccountId}", credential.LogicalAccountId);

        var refitSettings = new RefitSettings
        {
            ContentSerializer = new SystemTextJsonContentSerializer(_jsonSerializerOptions)
        };

        return RestService.For<IVkOrdApiClient>(httpClient, refitSettings);
    }

    private VkOrdApiContext GetApiContext(ApiCredential credential)
    {
        var token = _protector.Decrypt(credential.TokenEncrypted);
        return new VkOrdApiContext
        {
            ApiKey = token,
            Route = credential.ApiEnvironment
        };
    }

    /// <summary>
    /// Конвертер JSON для enum с поддержкой EnumMember
    /// </summary>
    private class EnumMemberJsonConverter : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert.IsEnum;
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            return (JsonConverter)Activator.CreateInstance(
                typeof(EnumMemberConverter<>).MakeGenericType(typeToConvert))!;
        }

        private class EnumMemberConverter<T> : JsonConverter<T> where T : struct, Enum
        {
            public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.String)
                {
                    var stringValue = reader.GetString();
                    if (stringValue != null)
                    {
                        foreach (var field in typeToConvert.GetFields())
                        {
                            if (field.FieldType == typeToConvert)
                            {
                                var enumMember = field.GetCustomAttribute<EnumMemberAttribute>();
                                if (enumMember != null && enumMember.Value == stringValue)
                                {
                                    return (T)field.GetValue(null)!;
                                }
                            }
                        }

                        if (Enum.TryParse<T>(stringValue, true, out var result))
                        {
                            return result;
                        }
                    }
                }

                throw new JsonException($"Unable to convert '{reader.GetString()}' to {typeToConvert.Name}");
            }

            public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
            {
                var field = value.GetType().GetField(value.ToString());
                if (field != null)
                {
                    var enumMember = field.GetCustomAttribute<EnumMemberAttribute>();
                    if (enumMember != null)
                    {
                        writer.WriteStringValue(enumMember.Value);
                        return;
                    }
                }

                writer.WriteStringValue(value.ToString());
            }
        }
    }
}

/// <summary>
/// Обработчик ошибок HTTP для фоновых задач
/// </summary>
public class BackgroundVkOrdApiErrorHandler : DelegatingHandler
{
    private readonly ILogger<BackgroundVkOrdApiErrorHandler> _logger;
    private const int MaxRetryAttempts = 3;
    private const int BaseDelayMs = 1000;

    public BackgroundVkOrdApiErrorHandler(ILogger<BackgroundVkOrdApiErrorHandler> logger)
    {
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Exception? lastException = null;

        for (int attempt = 0; attempt <= MaxRetryAttempts; attempt++)
        {
            try
            {
                var response = await base.SendAsync(request, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    return response;
                }

                if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                {
                    if (attempt < MaxRetryAttempts)
                    {
                        var delay = CalculateDelay(attempt);
                        _logger.LogWarning("Rate limit detected (attempt {Attempt}/{MaxAttempts}). Retrying in {Delay}ms",
                            attempt + 1, MaxRetryAttempts + 1, delay);

                        await Task.Delay(delay, cancellationToken);
                        continue;
                    }
                }

                var errorMessage = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("VK ORD API error: {StatusCode} - {Error}", response.StatusCode, errorMessage);
                throw new HttpRequestException($"VK ORD API error: {response.StatusCode} - {errorMessage}");
            }
            catch (Exception ex) when (ex is not HttpRequestException)
            {
                lastException = ex;
                _logger.LogError(ex, "Error during HTTP request (attempt {Attempt})", attempt + 1);

                if (attempt < MaxRetryAttempts)
                {
                    var delay = CalculateDelay(attempt);
                    await Task.Delay(delay, cancellationToken);
                    continue;
                }

                throw;
            }
        }

        if (lastException != null)
        {
            throw lastException;
        }

        throw new InvalidOperationException("Unexpected end of retry loop");
    }

    private static int CalculateDelay(int attempt)
    {
        return BaseDelayMs * (int)Math.Pow(2, attempt);
    }
}

/// <summary>
/// Обработчик заголовков HTTP для фоновых задач
/// </summary>
public class BackgroundVkOrdApiHeaderHandler : DelegatingHandler
{
    private readonly ILogger<BackgroundVkOrdApiHeaderHandler> _logger;

    public BackgroundVkOrdApiHeaderHandler(ILogger<BackgroundVkOrdApiHeaderHandler> logger)
    {
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        request.Headers.Add("X-Request-Timestamp", timestamp.ToString());
        request.Headers.Add("X-Request-Id", Guid.NewGuid().ToString("N")[..8]);

        if (request.Content != null)
        {
            request.Headers.Add("X-Content-Type", request.Content.Headers.ContentType?.ToString() ?? "application/json");
        }

        _logger.LogDebug("Added dynamic headers to background VK ORD API request: {RequestId}, {Timestamp}",
            request.Headers.GetValues("X-Request-Id").FirstOrDefault(), timestamp);

        return await base.SendAsync(request, cancellationToken);
    }
}
