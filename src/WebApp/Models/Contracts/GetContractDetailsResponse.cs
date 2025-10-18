using Domain.Entities.VkOrd;
using WebApp.Models.Common;

namespace WebApp.Models.Contracts;

/// <summary>
/// Ответ с деталями договора
/// </summary>
public class GetContractDetailsResponse : CacheResponse
{
    /// <summary>
    /// Договор
    /// </summary>
    public VkOrdContract? Contract { get; set; }

    /// <summary>
    /// Список креативов
    /// </summary>
    public List<CreativeDto> Creatives { get; set; } = new();

    /// <summary>
    /// Общее количество креативов
    /// </summary>
    public int TotalCreatives { get; set; }

    /// <summary>
    /// Количество возвращенных креативов
    /// </summary>
    public int ReturnedCreatives { get; set; }
}