using Domain.Entities;
using Domain.VkOrdApi;

namespace Jobs.Services.Interfaces;

/// <summary>
/// Фабрика для создания VK ORD API клиента в фоновых задачах (без HTTP контекста)
/// </summary>
public interface IBackgroundVkOrdApiClientFactory
{
    /// <summary>
    /// Создать клиент для VK ORD API с указанными credentials
    /// </summary>
    /// <param name="credential">API credentials для аутентификации</param>
    /// <returns>Клиент VK ORD API</returns>
    Task<IVkOrdApiClient> CreateClient(ApiCredential credential);
}
