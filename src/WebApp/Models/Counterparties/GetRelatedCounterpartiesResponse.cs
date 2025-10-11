using WebApp.Models.Common;

namespace WebApp.Models.Counterparties;

/// <summary>
/// Ответ со связанными контрагентами
/// </summary>
public class GetRelatedCounterpartiesResponse : CacheResponse
{
    /// <summary>
    /// Список связанных контрагентов
    /// </summary>
    public List<CounterpartyDto> RelatedCounterparties { get; set; } = new();

    /// <summary>
    /// Общее количество найденных связанных контрагентов
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Количество возвращенных связанных контрагентов
    /// </summary>
    public int ReturnedCount { get; set; }
}