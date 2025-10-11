using WebApp.Models.Common;

namespace WebApp.Models.Contracts;

/// <summary>
/// Ответ с договором между двумя контрагентами
/// </summary>
public class GetContractBetweenCounterpartiesResponse : CacheResponse
{
    /// <summary>
    /// Договор
    /// </summary>
    public ContractDto? Contract { get; set; }
}