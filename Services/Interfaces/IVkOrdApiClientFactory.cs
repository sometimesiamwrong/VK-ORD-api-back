using VkOrdApiWrapper.Models.VkOrd;

namespace VkOrdApiWrapper.Services.Interfaces;

/// <summary>
/// Фабрика для создания клиентов VK ОРД API с динамическими настройками
/// </summary>
public interface IVkOrdApiClientFactory
{
    /// <summary>
    /// Создать клиент для работы с VK ОРД API
    /// </summary>
    IVkOrdApiClient CreateClient(VkApiContext apiContext);
}



