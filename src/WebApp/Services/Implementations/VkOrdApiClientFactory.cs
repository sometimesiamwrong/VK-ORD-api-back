using Domain.Entities;
using Microsoft.Extensions.Logging;
using Refit;
using WebApp.Services.Interfaces;
using VkOrdApi;
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

namespace WebApp.Services.Implementations;

/// <summary>
/// Фабрика для создания клиентов VK ОРД API с динамическими настройками
/// </summary>
public class VkOrdApiClientFactory : IVkOrdApiClientFactory
{
    private readonly ILogger<VkOrdApiClientFactory> _logger;
    private readonly IGetApiCredentialByGuidRepository _getApiCredentialByGuidRepository;
    private readonly ISecretProtector _protector;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public VkOrdApiClientFactory(
        ILogger<VkOrdApiClientFactory> logger,
        IGetApiCredentialByGuidRepository getApiCredentialByGuidRepository,
        ISecretProtector protector,
        IHttpContextAccessor httpContextAccessor,
        JsonSerializerOptions jsonSerializerOptions)
    {
        _logger = logger;
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
    /// </summary>
    public async Task<IVkOrdApiClient> CreateClient()
    {
        var guid = _httpContextAccessor.GetVkOrdCredentialId();
        var apiContext = await GetApiContextAsync(guid);

        if (apiContext == null)
        {
            throw new ArgumentException("Invalid ApiCredential is null");
        }

        var baseUrl = apiContext.GetBaseUrl();

        _logger.LogInformation("Creating VK ORD API client for route: {Route}, base URL: {BaseUrl}",
            apiContext.Route, baseUrl);

        var httpClientHandler = new HttpClientHandler();
        var errorHandler = new VkOrdApiErrorHandler();
        errorHandler.InnerHandler = httpClientHandler;

        var httpClient = new HttpClient(errorHandler)
        {
            BaseAddress = new Uri(baseUrl),
            Timeout = TimeSpan.FromSeconds(30)
        };

        // Устанавливаем заголовки аутентификации
        httpClient.DefaultRequestHeaders.Add("Authorization", apiContext.GetAuthorizationHeader());
        httpClient.DefaultRequestHeaders.Add("User-Agent", "WebApp/1.0");

        // Настраиваем сериализацию для Refit с поддержкой EnumMember
        var refitSettings = new RefitSettings
        {
            ContentSerializer = new SystemTextJsonContentSerializer(_jsonSerializerOptions)
        };

        // Создаем Refit клиент
        return RestService.For<IVkOrdApiClient>(httpClient, refitSettings);
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

    private async Task<VkApiContext?> GetApiContextAsync(Guid guid)
    {
        var apiCredential = await _getApiCredentialByGuidRepository.GetByGuidAsync(guid);
        if (apiCredential == null)
        {
            return null;
        }

        var token = _protector.Decrypt(apiCredential.TokenEncrypted);
        return new VkApiContext
        {
            ApiKey = token,
            Route = apiCredential.Environment
        };
    }
}

public class VkOrdApiErrorHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await base.SendAsync(request, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return response;
            }

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
                // Игнорируем ошибки чтения контента (например, если тело пустое или не JSON)
                // Можно добавить логирование здесь, если инжектировать ILogger
            }

            var brokenRule = new BrokenRule((long)BrokenRuleCodes.VkOrdApiError, errorMessage, "ExternalApi");
            var brokenRules = new BrokenRulesCollection(brokenRule);
            throw new BrokenRulesException(brokenRules);
        }
        catch (Refit.ApiException ex)
        {
            var brokenRule = new BrokenRule((long)BrokenRuleCodes.VkOrdApiError, ex.Content ?? "Ошибка VK ОРД API", "ExternalApi");
            var brokenRules = new BrokenRulesCollection(brokenRule);
            throw new BrokenRulesException(brokenRules);
        }
    }
}
