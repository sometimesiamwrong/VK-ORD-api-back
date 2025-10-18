using WebApp.Models.Responses;

namespace WebApp.Repositories.Interfaces.VkOrd.Statistics;

/// <summary>
/// Репозиторий для получения списка статистик
/// </summary>
public interface IGetStatisticsListRepository
{
    /// <summary>
    /// Получает список статистик с пагинацией и фильтрацией
    /// </summary>
    /// <param name="creativeExternalId">Фильтр по креативу (опционально)</param>
    /// <param name="padExternalId">Фильтр по площадке (опционально)</param>
    /// <param name="offset">Смещение для пагинации</param>
    /// <param name="limit">Лимит элементов</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task<GetStatisticsDto> GetListAsync(
        string? creativeExternalId = null,
        string? padExternalId = null,
        int offset = 0,
        int limit = 100,
        CancellationToken cancellationToken = default);
}
