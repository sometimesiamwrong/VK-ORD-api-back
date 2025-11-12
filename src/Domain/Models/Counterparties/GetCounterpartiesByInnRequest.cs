using System.ComponentModel.DataAnnotations;
using Domain.Models.Common;

namespace Domain.Models.Counterparties;

/// <summary>
/// Запрос на получение контрагентов по ИНН
/// </summary>
public class GetCounterpartiesByInnRequest : CacheRequest
{
    /// <summary>
    /// ИНН контрагента
    /// </summary>
    [Required]
    [StringLength(12, MinimumLength = 10)]
    public string Inn { get; set; } = string.Empty;

    /// <summary>
    /// Включить связанные данные (договоры, креативы)
    /// </summary>
    public bool IncludeRelatedData { get; set; } = false;

    /// <summary>
    /// Максимальное количество результатов
    /// </summary>
    [Range(1, 1000)]
    public int MaxResults { get; set; } = 100;
}
