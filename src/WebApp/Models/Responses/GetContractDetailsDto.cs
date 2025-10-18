using Domain.Entities.VkOrd;
using Domain.VkOrdApi.Contract;

namespace WebApp.Models.Responses;

/// <summary>
/// DTO с полными данными контракта (включая все связанные данные)
/// </summary>
public class GetContractDetailsDto
{
    /// <summary>
    /// Основные данные контракта из VK ORD API
    /// </summary>
    public VkOrdApiContractResponse? Contract { get; set; }

    /// <summary>
    /// Стороны контракта (полные данные контрагентов)
    /// </summary>
    public List<VkOrdCounterparty> Parties { get; set; } = new();

    /// <summary>
    /// Креативы, связанные с контрактом (полные данные с медиа)
    /// </summary>
    public List<VkOrdCreative> Creatives { get; set; } = new();

    /// <summary>
    /// Дополнительные соглашения (доп. договоры)
    /// </summary>
    public List<AdditionalContractDto> AdditionalContracts { get; set; } = new();

    /// <summary>
    /// Статус синхронизации
    /// </summary>
    public string SyncStatus { get; set; } = string.Empty;

    /// <summary>
    /// Дата создания
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Дата последнего обновления
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }
}

/// <summary>
/// DTO дополнительного соглашения
/// </summary>
public class AdditionalContractDto
{
    /// <summary>
    /// External ID дополнительного соглашения
    /// </summary>
    public string ExternalId { get; set; } = string.Empty;

    /// <summary>
    /// Серийный номер
    /// </summary>
    public string? Serial { get; set; }

    /// <summary>
    /// Дата заключения
    /// </summary>
    public DateTime? Date { get; set; }

    /// <summary>
    /// Статус синхронизации
    /// </summary>
    public string SyncStatus { get; set; } = string.Empty;
}
