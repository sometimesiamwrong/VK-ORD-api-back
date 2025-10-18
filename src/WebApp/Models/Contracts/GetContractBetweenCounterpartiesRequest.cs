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
    public string ClientExternalId { get; set; } = string.Empty;

    /// <summary>
    /// ИНН второго контрагента (подрядчика)
    /// </summary>
    [Required]
    public string ContractorExternalId { get; set; } = string.Empty;
}