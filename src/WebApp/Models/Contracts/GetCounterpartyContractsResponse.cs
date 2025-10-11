using WebApp.Models.Common;

namespace WebApp.Models.Contracts;

/// <summary>
/// Ответ с договорами контрагента
/// </summary>
public class GetCounterpartyContractsResponse : CacheResponse
{
    /// <summary>
    /// Список договоров
    /// </summary>
    public List<ContractDto> Contracts { get; set; } = new();

    /// <summary>
    /// Общее количество найденных договоров
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Количество возвращенных договоров
    /// </summary>
    public int ReturnedCount { get; set; }
}
