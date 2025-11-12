using System.ComponentModel.DataAnnotations;
using Domain.VkOrdApi.Statistics;

namespace Domain.Models.Requests;

/// <summary>
/// Запрос на создание/обновление статистики
/// </summary>
public class CreateOrUpdateStatisticsRequest
{
    /// <summary>
    /// Массив элементов статистики
    /// </summary>
    [Required]
    [MinLength(1)]
    public List<VkOrdApiStatisticsItem> Items { get; set; } = new();
}
