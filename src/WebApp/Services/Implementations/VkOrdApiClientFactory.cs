using Domain.Entities;
using Microsoft.Extensions.Logging;
using Refit;
using WebApp.Services.Interfaces;
using WebApp.Repositories.Interfaces.ApiCredentials;
using WebApp.Security;
using Microsoft.AspNetCore.Http;
using Domain.Extensions;
using System.Net.Http;
using Domain.BrokenRules;
using Domain.Exceptions;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Encodings.Web;
using System.Runtime.Serialization;
using System.Reflection;
using Domain.VkOrdApi;

namespace WebApp.Services.Implementations;

/// <summary>
/// Фабрика для создания клиентов VK ОРД API с динамическими настройками
/// </summary>
public class VkOrdApiClientFactory : IVkOrdApiClientFactory
{
    private readonly ILogger<VkOrdApiClientFactory> _logger;
    private readonly ILoggerFactory _loggerFactory;
    private readonly IGetApiCredentialByGuidRepository _getApiCredentialByGuidRepository;
    private readonly ISecretProtector _protector;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public VkOrdApiClientFactory(
        ILogger<VkOrdApiClientFactory> logger,
        ILoggerFactory loggerFactory,
        IGetApiCredentialByGuidRepository getApiCredentialByGuidRepository,
        ISecretProtector protector,
        IHttpContextAccessor httpContextAccessor,
        JsonSerializerOptions jsonSerializerOptions)
    {
        _logger = logger;
        _loggerFactory = loggerFactory;
        _getApiCredentialByGuidRepository = getApiCredentialByGuidRepository;
        _protector = protector;
        _httpContextAccessor = httpContextAccessor;

        // Создаем копию настроек сериализации и добавляем конвертер для enum с EnumMember
        _jsonSerializerOptions = new JsonSerializerOptions(jsonSerializerOptions);
        // Добавляем наш конвертер в начало списка, чтобы он имел приоритет над стандартным JsonStringEnumConverter
        _jsonSerializerOptions.Converters.Insert(0, new EnumMemberJsonConverter());
    }

    /// <summary>
    /// Создать клиент для работы с VK ОРД API
    /// Кэширует HttpClient на уровне HTTP-запроса для оптимизации производительности
    /// </summary>
    public async Task<IVkOrdApiClient> CreateClient()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            throw new InvalidOperationException("HttpContext is not available.");
        }

        // Ключ для кэширования клиента в рамках одного HTTP-запроса
        const string clientCacheKey = "VkOrdApiClient";
        
        // Проверяем, есть ли уже кэшированный клиент для этого запроса
        if (httpContext.Items.TryGetValue(clientCacheKey, out var cachedClient) && cachedClient is IVkOrdApiClient client)
        {
            return client;
        }

        var apiContext = await GetApiContextAsync();

        if (apiContext == null)
        {
            throw new ArgumentException("Invalid ApiCredential is null");
        }

        var baseUrl = apiContext.GetBaseUrl();

        var httpClientHandler = new HttpClientHandler()
        {
            // Настройки для оптимальной производительности
            MaxConnectionsPerServer = 10,
            UseCookies = false, // VK ORD API не использует cookies
            AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate | System.Net.DecompressionMethods.Brotli,
            UseProxy = false // Отключаем прокси для прямого соединения
        };
        
        var headerHandler = new VkOrdApiHeaderHandler(_loggerFactory.CreateLogger<VkOrdApiHeaderHandler>());
        var errorHandler = new VkOrdApiErrorHandler(_loggerFactory.CreateLogger<VkOrdApiErrorHandler>());
        
        headerHandler.InnerHandler = httpClientHandler;
        errorHandler.InnerHandler = headerHandler;

        var httpClient = new HttpClient(errorHandler)
        {
            BaseAddress = new Uri(baseUrl),
            Timeout = TimeSpan.FromSeconds(30),
            MaxResponseContentBufferSize = 10 * 1024 * 1024 // 10MB буфер для больших ответов
        };

        // Устанавливаем заголовки для оптимальной работы с VK ORD API
        httpClient.DefaultRequestHeaders.Add("Authorization", apiContext.GetAuthorizationHeader());
        httpClient.DefaultRequestHeaders.Add("User-Agent", "VK-ORD-API-Wrapper/1.0 (WebApp)");
        httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        httpClient.DefaultRequestHeaders.Add("Accept-Language", "ru-RU,ru;q=0.9,en;q=0.8");
        httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
        httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
        httpClient.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
        httpClient.DefaultRequestHeaders.Add("Pragma", "no-cache");
        
        // Добавляем заголовки для идентификации клиента
        httpClient.DefaultRequestHeaders.Add("X-Client-Version", "1.0.0");
        httpClient.DefaultRequestHeaders.Add("X-Client-Type", "WebApp");
        httpClient.DefaultRequestHeaders.Add("X-API-Route", apiContext.Route.ToString());

        // Логируем настроенные заголовки для отладки
        //_logger.LogDebug("VK ORD API client headers configured: User-Agent={UserAgent}, Accept={Accept}, Route={Route}",
        //    "VK-ORD-API-Wrapper/1.0 (WebApp)", "application/json", apiContext.Route);
        var credi = await GetVkOrdCredentialAsync();
        var accountId = credi.LogicalAccountId;
        _logger.LogDebug($"LogicalAccount: {accountId}", accountId);

        // Настраиваем сериализацию для Refit с поддержкой EnumMember
        var refitSettings = new RefitSettings
        {
            ContentSerializer = new SystemTextJsonContentSerializer(_jsonSerializerOptions)
        };

        // Создаем Refit клиент
        var refitClient = RestService.For<IVkOrdApiClient>(httpClient, refitSettings);
        
        // Кэшируем клиент в HttpContext.Items для повторного использования в рамках этого запроса
        httpContext.Items[clientCacheKey] = refitClient;
        
        // Регистрируем очистку ресурсов при завершении запроса
        httpContext.Response.RegisterForDispose(httpClient);
        
        return refitClient;
    }

    /// <summary>
    /// Конвертер JSON для enum, который использует значения из атрибута EnumMember
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
                        // Ищем enum значение по EnumMember атрибуту
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

                        // Если не нашли по EnumMember, пробуем стандартный парсинг
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

                // Если нет EnumMember атрибута, используем имя enum
                writer.WriteStringValue(value.ToString());
            }
        }
    }

    private async Task<VkOrdApiContext> GetApiContextAsync()
    {
        var apiCredential = await GetVkOrdCredentialAsync();

        var token = _protector.Decrypt(apiCredential.TokenEncrypted);
        return new VkOrdApiContext
        {
            ApiKey = token,
            Route = apiCredential.ApiEnvironment
        };
    }

    public async Task<ApiCredential> GetVkOrdCredentialAsync()
    {
        var userId = _httpContextAccessor.HttpContext?.User.GetUserId();
        if (userId == null)
        {
            throw new UnauthorizedAccessException("User is not authenticated");
        }
        var apiCredential = await _getApiCredentialByGuidRepository.GetByGuidAsync(_httpContextAccessor.GetVkOrdCredentialId());
        if (apiCredential == null || apiCredential.UserId != userId)
        {
            throw BrokenRuleCodes.VkOrdCredentialsNotFound.AsExn();
        }
        return apiCredential;
    }
}

public class VkOrdApiErrorHandler : DelegatingHandler
{
    private readonly ILogger<VkOrdApiErrorHandler> _logger;
    private const int MaxRetryAttempts = 3;
    private const int BaseDelayMs = 50; // 50 миллисекунд

    public VkOrdApiErrorHandler(ILogger<VkOrdApiErrorHandler> logger)
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

                // Проверяем на rate limit (429)
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
                    else
                    {
                        _logger.LogError("Rate limit exceeded after {MaxAttempts} attempts", MaxRetryAttempts + 1);
                        var rateLimitRule = new BrokenRule((long)BrokenRuleCodes.VkOrdApiRateLimit, 
                            "Превышен лимит запросов к VK ОРД API. Попробуйте позже.", "ExternalApi");
                        var rateLimitRules = new BrokenRulesCollection(rateLimitRule);
                        throw new BrokenRulesException(rateLimitRules);
                    }
                }

                // Обработка других ошибок
                string errorMessage = response.ReasonPhrase ?? response.StatusCode.ToString();

                try
                {
                    var content = await response.Content.ReadAsStringAsync(cancellationToken);
                    if (!string.IsNullOrWhiteSpace(content))
                    {
                        errorMessage = content;
                    }
                }
                catch
                {
                    // Игнорируем ошибки чтения контента
                }

                var errorRule = new BrokenRule((long)BrokenRuleCodes.VkOrdApiError, errorMessage, "ExternalApi");
                var errorRules = new BrokenRulesCollection(errorRule);
                throw new BrokenRulesException(errorRules);
            }
            catch (Refit.ApiException ex)
            {
                lastException = ex;
                
                // Проверяем на rate limit в Refit исключении
                if (ex.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                {
                    if (attempt < MaxRetryAttempts)
                    {
                        var delay = CalculateDelay(attempt);
                        _logger.LogWarning("Rate limit detected in Refit exception (attempt {Attempt}/{MaxAttempts}). Retrying in {Delay}ms", 
                            attempt + 1, MaxRetryAttempts + 1, delay);
                        
                        await Task.Delay(delay, cancellationToken);
                        continue;
                    }
                    else
                    {
                        _logger.LogError("Rate limit exceeded after {MaxAttempts} attempts", MaxRetryAttempts + 1);
                        var refitRateLimitRule = new BrokenRule((long)BrokenRuleCodes.VkOrdApiRateLimit, 
                            "Превышен лимит запросов к VK ОРД API. Попробуйте позже.", "ExternalApi");
                        var refitRateLimitRules = new BrokenRulesCollection(refitRateLimitRule);
                        throw new BrokenRulesException(refitRateLimitRules);
                    }
                }

                // Другие Refit ошибки
                var refitErrorRule = new BrokenRule((long)BrokenRuleCodes.VkOrdApiError, ex.Content ?? "Ошибка VK ОРД API", "ExternalApi");
                var refitErrorRules = new BrokenRulesCollection(refitErrorRule);
                throw new BrokenRulesException(refitErrorRules);
            }
            catch (BrokenRulesException)
            {
                // Пробрасываем BrokenRulesException без изменений
                throw;
            }
            catch (Exception ex)
            {
                lastException = ex;
                _logger.LogError(ex, "Unexpected error during HTTP request (attempt {Attempt})", attempt + 1);
                
                if (attempt < MaxRetryAttempts)
                {
                    var delay = CalculateDelay(attempt);
                    await Task.Delay(delay, cancellationToken);
                    continue;
                }
                
                throw;
            }
        }

        // Если дошли сюда, значит все попытки исчерпаны
        if (lastException != null)
        {
            throw lastException;
        }

        throw new InvalidOperationException("Unexpected end of retry loop");
    }

    /// <summary>
    /// Вычисляет задержку для повторной попытки с экспоненциальным увеличением
    /// </summary>
    private static int CalculateDelay(int attempt)
    {
        // Экспоненциальная задержка: 1s, 2s, 4s
        return BaseDelayMs * (int)Math.Pow(2, attempt);
    }
}

/// <summary>
/// Обработчик для добавления динамических заголовков к запросам VK ORD API
/// </summary>
public class VkOrdApiHeaderHandler : DelegatingHandler
{
    private readonly ILogger<VkOrdApiHeaderHandler> _logger;

    public VkOrdApiHeaderHandler(ILogger<VkOrdApiHeaderHandler> logger)
    {
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Добавляем динамические заголовки для каждого запроса
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        request.Headers.Add("X-Request-Timestamp", timestamp.ToString());
        request.Headers.Add("X-Request-Id", Guid.NewGuid().ToString("N")[..8]);
        
        // Добавляем заголовок для отслеживания запросов
        if (request.Content != null)
        {
            request.Headers.Add("X-Content-Type", request.Content.Headers.ContentType?.ToString() ?? "application/json");
        }

        _logger.LogDebug("Added dynamic headers to VK ORD API request: {RequestId}, {Timestamp}", 
            request.Headers.GetValues("X-Request-Id").FirstOrDefault(), timestamp);

        return await base.SendAsync(request, cancellationToken);
    }
}
