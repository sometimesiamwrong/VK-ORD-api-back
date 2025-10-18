using Domain.VkOrdApi.Statistics;

namespace WebApp.Models.Responses;

/// <summary>
/// Ответ со списком статистик
/// </summary>
public class GetStatisticsDto
{
    /// <summary>
    /// Список элементов статистики
    /// </summary>
    public List<VkOrdApiStatisticsItem> Data { get; set; } = new();

    /// <summary>
    /// Общее количество элементов
    /// </summary>
    public int TotalItemsCount { get; set; }

    /// <summary>
    /// Лимит элементов за запрос
    /// </summary>
    public int Limit { get; set; }
}
