using Domain.Entities.VkOrd;
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
    public VkOrdContract? Contract { get; set; }
}