using Domain.Entities.VkOrd;
using Domain.Models.Common;

namespace Domain.Models.Contracts;

/// <summary>
/// Ответ с договором между двумя контрагентами
/// </summary>
public class GetContractBetweenCounterpartiesResponse : CacheResponse
{
    /// <summary>
    /// Договор
    /// </summary>
    public VkOrdContract? Contract { get; set; }
}