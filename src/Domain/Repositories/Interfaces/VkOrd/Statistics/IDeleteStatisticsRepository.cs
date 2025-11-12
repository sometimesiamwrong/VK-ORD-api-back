namespace Domain.Repositories.Interfaces.VkOrd.Statistics;

/// <summary>
/// Репозиторий для удаления статистики
/// </summary>
public interface IDeleteStatisticsRepository
{
    /// <summary>
    /// Удаляет статистику из VK ORD API и локальной БД
    /// </summary>
    /// <param name="creativeExternalId">Внешний ID креатива</param>
    /// <param name="padExternalId">Внешний ID площадки</param>
    /// <param name="dateStartActual">Фактическая дата начала</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task DeleteAsync(
        string creativeExternalId,
        string padExternalId,
        string dateStartActual,
        CancellationToken cancellationToken);
}
