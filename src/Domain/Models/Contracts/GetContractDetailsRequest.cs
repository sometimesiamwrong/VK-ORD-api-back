using System.ComponentModel.DataAnnotations;
using Domain.Models.Common;

namespace Domain.Models.Contracts;

/// <summary>
/// Запрос на получение деталей договора
/// </summary>
public class GetContractDetailsRequest : CacheRequest
{
    /// <summary>
    /// Внешний идентификатор договора
    /// </summary>
    [Required]
    public string ContractExternalId { get; set; } = string.Empty;

    /// <summary>
    /// Включать ли креативы в ответ
    /// </summary>
    public bool IncludeCreatives { get; set; } = true;

    /// <summary>
    /// Максимальное количество креативов
    /// </summary>
    [Range(1, 1000)]
    public int MaxCreatives { get; set; } = 100;
}