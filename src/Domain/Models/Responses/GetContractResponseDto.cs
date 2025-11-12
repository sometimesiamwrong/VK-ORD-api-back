using Domain.Entities.VkOrd;

namespace Domain.Models.Responses;

/// <summary>
/// DTO для ответа с данными контракта
/// </summary>
public class GetContractsDto
{
    /// <summary>
    /// Данные контракта
    /// </summary>
    public List<VkOrdContract> Data { get; set; } = new();

    /// <summary>
    /// Общее количество элементов в VK ORD
    /// </summary>
    public int TotalItemsCount { get; set; }

    public int TotalCount => Data?.Count ?? 0;

    /// <summary>
    /// Лимит элементов за запрос
    /// </summary>
    public int Limit { get; set; }
}