using Microsoft.Extensions.Options;
using Refit;
using VkOrdApiWrapper.Configuration;
using VkOrdApiWrapper.Models.VkOrd;
using VkOrdApiWrapper.Services.Interfaces;
using VkOrdApiWrapper.Data;
using Microsoft.EntityFrameworkCore;
using VkOrdApiWrapper.Extensions;
using VkOrdApiWrapper.Security;

namespace VkOrdApiWrapper.Services.Implementations;

/// <summary>
/// Фабрика для создания клиентов VK ОРД API с динамическими настройками
/// </summary>
public class VkOrdApiClientFactory : IVkOrdApiClientFactory
{
    private readonly VkOrdConfiguration _defaultConfig;
    private readonly ILogger<VkOrdApiClientFactory> _logger;
    private readonly ApplicationDbContext _db;
    private readonly ISecretProtector _protector;

    public VkOrdApiClientFactory(
        IOptions<VkOrdConfiguration> config,
        ILogger<VkOrdApiClientFactory> logger,
        ApplicationDbContext db,
        ISecretProtector protector)
    {
        _defaultConfig = config.Value;
        _logger = logger;
        _db = db;
        _protector = protector;
    }

    /// <summary>
    /// Создать клиент для работы с VK ОРД API
    /// </summary>
    public IVkOrdApiClient CreateClient(VkApiContext apiContext)
    {
        if (!apiContext.IsValid())
        {
            throw new ArgumentException("Invalid VkApiContext: ApiKey and Route are required");
        }

        var baseUrl = apiContext.GetBaseUrl();

        _logger.LogInformation("Creating VK ORD API client for route: {Route}, base URL: {BaseUrl}",
            apiContext.Route, baseUrl);

        var handler = new HttpClientHandler();

        // Настройка прокси если нужно (пока отключено)
        // TODO: добавить настройку прокси в конфигурацию если необходимо

        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri(baseUrl),
            Timeout = TimeSpan.FromSeconds(_defaultConfig.TimeoutSeconds)
        };

        // Устанавливаем заголовки аутентификации
        httpClient.DefaultRequestHeaders.Add("Authorization", apiContext.GetAuthorizationHeader());
        httpClient.DefaultRequestHeaders.Add("User-Agent", "VkOrdApiWrapper/1.0");

        // Создаем Refit клиент
        return RestService.For<IVkOrdApiClient>(httpClient);
    }
}
