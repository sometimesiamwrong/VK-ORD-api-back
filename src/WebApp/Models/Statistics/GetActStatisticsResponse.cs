using WebApp.Models.Common;

namespace WebApp.Models.Statistics;

/// <summary>
/// Ответ со статистикой актов
/// </summary>
public class GetActStatisticsResponse : CacheResponse
{
    /// <summary>
    /// Список статистики
    /// </summary>
    //public List<StatisticsDto> Statistics { get; set; } = new();

    /// <summary>
    /// Общее количество найденных записей
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Количество возвращенных записей
    /// </summary>
    public int ReturnedCount { get; set; }

    /// <summary>
    /// Общая сумма
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Общее количество показов
    /// </summary>
    public long TotalShows { get; set; }
}