using Domain.Entities.VkOrd;
using Domain.VkOrdApi.Person;

namespace Domain.Models.Contracts;

/// <summary>
/// DTO договора
/// </summary>
public class CounterpartyWithContractsDto
{
    /// <summary>
    /// Внешний идентификатор контрагента
    /// </summary>
    public string ExternalId { get; set; } = string.Empty;

    /// <summary>
    /// Данные контрагента
    /// </summary>
    public VkOrdApiPersonResponse Data { get; set; } = new();

    /// <summary>
    /// Список договоров
    /// </summary>
    public List<VkOrdContract> Contracts { get; set; } = new();
}