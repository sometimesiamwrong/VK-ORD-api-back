using System.ComponentModel.DataAnnotations;
using WebApp.Models.Common;

namespace WebApp.Models.Contracts;

/// <summary>
/// Запрос на получение договоров контрагента
/// </summary>
public class GetCounterpartyContractsRequest : CacheRequest
{
    /// <summary>
    /// ИНН контрагента
    /// </summary>
    [Required]
    [StringLength(12, MinimumLength = 10)]
    public string Inn { get; set; } = string.Empty;

    /// <summary>
    /// Максимальное количество результатов
    /// </summary>
    [Range(1, 1000)]
    public int MaxResults { get; set; } = 100;

    /// <summary>
    /// Включить дополнительные соглашения
    /// </summary>
    public bool IncludeAdditionalContracts { get; set; } = true;
}

