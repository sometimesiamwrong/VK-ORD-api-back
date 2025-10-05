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

    public VkOrdApiClientFactory(
        ILogger<VkOrdApiClientFactory> logger,
        IGetApiCredentialByGuidRepository getApiCredentialByGuidRepository,
        ISecretProtector protector,
        IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _getApiCredentialByGuidRepository = getApiCredentialByGuidRepository;
        _protector = protector;
        _httpContextAccessor = httpContextAccessor;
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

        // Создаем Refit клиент
        return RestService.For<IVkOrdApiClient>(httpClient);
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
