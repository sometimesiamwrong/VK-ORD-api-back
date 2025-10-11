using System;
using System.Collections.Generic;

namespace WebApp.Models.Contracts;

/// <summary>
/// DTO договора
/// </summary>
public class ContractDto
{
    /// <summary>
    /// Внешний идентификатор договора
    /// </summary>
    public string ExternalId { get; set; } = string.Empty;

    /// <summary>
    /// Серийный номер договора
    /// </summary>
    public string? Serial { get; set; }

    /// <summary>
    /// Тип договора
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Внешний идентификатор клиента
    /// </summary>
    public string? ClientExternalId { get; set; }

    /// <summary>
    /// Внешний идентификатор подрядчика
    /// </summary>
    public string? ContractorExternalId { get; set; }

    /// <summary>
    /// Внешний идентификатор родительского договора
    /// </summary>
    public string? ParentContractExternalId { get; set; }

    /// <summary>
    /// Сумма договора
    /// </summary>
    public decimal? Amount { get; set; }

    /// <summary>
    /// Дата договора
    /// </summary>
    public DateTime? Date { get; set; }

    /// <summary>
    /// Дата окончания договора
    /// </summary>
    public DateTime? DateEnd { get; set; }

    /// <summary>
    /// Дата последнего обновления в VK ORD
    /// </summary>
    public DateTimeOffset? LastUpdated { get; set; }

    /// <summary>
    /// Статус синхронизации
    /// </summary>
    public string SyncStatus { get; set; } = string.Empty;
}