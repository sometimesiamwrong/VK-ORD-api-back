using System.ComponentModel.DataAnnotations;
using WebApp.Models.Common;

namespace WebApp.Models.Counterparties;

/// <summary>
/// Запрос на получение связанных контрагентов
/// </summary>
public class GetRelatedCounterpartiesRequest : CacheRequest
{
    /// <summary>
    /// Внешний идентификатор контрагента
    /// </summary>
    [Required]
    public string ExternalId { get; set; } = string.Empty;

    /// <summary>
    /// Максимальное количество результатов
    /// </summary>
    [Range(1, 1000)]
    public int MaxResults { get; set; } = 100;

    /// <summary>
    /// Типы связей для фильтрации
    /// </summary>
    public List<string> RelationTypes { get; set; } = new();
}
