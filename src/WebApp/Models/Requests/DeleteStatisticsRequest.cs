using System.ComponentModel.DataAnnotations;

namespace WebApp.Models.Requests;

/// <summary>
/// Запрос на удаление статистики
/// </summary>
public class DeleteStatisticsRequest
{
    /// <summary>
    /// Массив элементов для удаления
    /// </summary>
    [Required]
    [MinLength(1)]
    public List<DeleteStatisticsItemRequest> Items { get; set; } = new();
}

/// <summary>
/// Элемент для удаления статистики
/// </summary>
public class DeleteStatisticsItemRequest
{
    /// <summary>
    /// Внешний идентификатор креатива
    /// </summary>
    [Required]
    public string CreativeExternalId { get; set; } = string.Empty;

    /// <summary>
    /// Внешний идентификатор рекламной площадки
    /// </summary>
    [Required]
    public string PadExternalId { get; set; } = string.Empty;

    /// <summary>
    /// Фактическая дата начала кампании (YYYY-MM-DD)
    /// </summary>
    [Required]
    public string DateStartActual { get; set; } = string.Empty;
}
