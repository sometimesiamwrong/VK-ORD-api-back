using VkOrdApi;

namespace WebApp.Services.Interfaces;

/// <summary>
/// Фабрика для создания клиентов VK ОРД API с динамическими настройками
/// </summary>
public interface IVkOrdApiClientFactory
{
    /// <summary>
    /// Создать клиент для работы с VK ОРД API
    /// </summary>
    Task<IVkOrdApiClient> CreateClient();
}



