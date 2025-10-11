using System.ComponentModel.DataAnnotations;
using WebApp.Models.Common;

namespace WebApp.Models.Statistics;

/// <summary>
/// Запрос на получение статистики актов
/// </summary>
public class GetActStatisticsRequest : CacheRequest
{
    /// <summary>
    /// Дата начала периода
    /// </summary>
    [Required]
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Дата окончания периода
    /// </summary>
    [Required]
    public DateTime EndDate { get; set; }

    /// <summary>
    /// ИНН контрагента (опционально)
    /// </summary>
    public string? CounterpartyInn { get; set; }

    /// <summary>
    /// Внешний идентификатор договора (опционально)
    /// </summary>
    public string? ContractExternalId { get; set; }

    /// <summary>
    /// Внешний идентификатор креатива (опционально)
    /// </summary>
    public string? CreativeExternalId { get; set; }

    /// <summary>
    /// Максимальное количество результатов
    /// </summary>
    [Range(1, 10000)]
    public int MaxResults { get; set; } = 1000;
}