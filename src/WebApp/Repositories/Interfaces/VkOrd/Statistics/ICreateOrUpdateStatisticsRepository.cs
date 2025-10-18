using Domain.VkOrdApi.Statistics;

namespace WebApp.Repositories.Interfaces.VkOrd.Statistics;

/// <summary>
/// Репозиторий для создания/обновления статистики
/// </summary>
public interface ICreateOrUpdateStatisticsRepository
{
    /// <summary>
    /// Создает или обновляет статистику в VK ORD API и локальной БД
    /// </summary>
    /// <param name="items">Список элементов статистики</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task CreateOrUpdateAsync(
        List<VkOrdApiStatisticsItem> items,
        CancellationToken cancellationToken = default);
}
