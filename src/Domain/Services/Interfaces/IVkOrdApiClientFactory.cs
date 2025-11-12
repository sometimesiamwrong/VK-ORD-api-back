using Domain.Entities;
using Domain.VkOrdApi;

namespace Domain.Services.Interfaces;

/// <summary>
/// Фабрика для создания клиентов VK ОРД API с динамическими настройками
/// </summary>
public interface IVkOrdApiClientFactory
{
    /// <summary>
    /// Создать клиент для работы с VK ОРД API (из HTTP context)
    /// </summary>
    Task<IVkOrdApiClient> CreateClient();

    /// <summary>
    /// Создать клиент для работы с VK ОРД API с явным credential (для Jobs)
    /// </summary>
    Task<IVkOrdApiClient> CreateClient(ApiCredential credential);

    /// <summary>
    /// Получить контекст API (из HTTP context)
    /// </summary>
    Task<ApiCredential> GetVkOrdCredentialAsync();


    /// <summary>
    /// Установить credential для текущего async потока (для Jobs)
    /// Используется через using для автоматической очистки
    /// </summary>
    IDisposable SetCredentialContext(ApiCredential credential);
}



