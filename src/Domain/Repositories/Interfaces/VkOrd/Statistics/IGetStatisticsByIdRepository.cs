using Domain.Entities.VkOrd;

namespace Domain.Repositories.Interfaces.VkOrd.Statistics;

/// <summary>
/// Репозиторий для получения статистики по ID
/// </summary>
public interface IGetStatisticsByIdRepository
{
    /// <summary>
    /// Получить статистику по external ID
    /// </summary>
    /// <param name="externalId">External ID статистики</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <param name="noCache">Игнорировать кэш и получить данные из API</param>
    /// <returns>Статистика</returns>
    Task<VkOrdStatistic> Get(string externalId, CancellationToken cancellationToken, bool noCache = false);
}
