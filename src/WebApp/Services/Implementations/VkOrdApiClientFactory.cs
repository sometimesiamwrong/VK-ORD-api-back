using Domain.Entities;
using Microsoft.Extensions.Logging;
using Refit;
using WebApp.Services.Interfaces;
using VkOrdApi.Contract;
using WebApp.Repositories.Interfaces.ApiCredentials;
using WebApp.Security;
using Microsoft.AspNetCore.Http;
using Domain.Extensions;

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
    public async Task<IVkOrdApiClient> CreateClientAsync()
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

        var handler = new HttpClientHandler();


        var httpClient = new HttpClient(handler)
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

    private async Task<VkApiContext> GetApiContextAsync(Guid guid)
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
