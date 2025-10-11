using System.ComponentModel.DataAnnotations;
using WebApp.Models.Common;

namespace WebApp.Models.Contracts;

/// <summary>
/// Запрос на получение договора между двумя контрагентами
/// </summary>
public class GetContractBetweenCounterpartiesRequest : CacheRequest
{
    /// <summary>
    /// ИНН первого контрагента (клиента)
    /// </summary>
    [Required]
    [StringLength(12, MinimumLength = 10)]
    public string ClientInn { get; set; } = string.Empty;

    /// <summary>
    /// ИНН второго контрагента (подрядчика)
    /// </summary>
    [Required]
    [StringLength(12, MinimumLength = 10)]
    public string ContractorInn { get; set; } = string.Empty;
}