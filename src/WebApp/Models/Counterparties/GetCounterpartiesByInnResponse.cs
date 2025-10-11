using WebApp.Models.Common;

namespace WebApp.Models.Counterparties;

/// <summary>
/// Ответ с контрагентами по ИНН
/// </summary>
public class GetCounterpartiesByInnResponse : CacheResponse
{
    /// <summary>
    /// Список контрагентов
    /// </summary>
    public List<CounterpartyDto> Counterparties { get; set; } = new();

    /// <summary>
    /// Общее количество найденных контрагентов
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Количество возвращенных контрагентов
    /// </summary>
    public int ReturnedCount { get; set; }
}

/// <summary>
/// Источник данных
/// </summary>
public enum DataSource
{
    /// <summary>
    /// Из кэша
    /// </summary>
    Cache = 0,

    /// <summary>
    /// Из API VK ORD
    /// </summary>
    Api = 1,

    /// <summary>
    /// Смешанный (кэш + API)
    /// </summary>
    Mixed = 2
}
