using System.ComponentModel;
using System.Runtime.Serialization;

namespace Domain.Entities.Enums;

/// <summary>
/// </summary>
public enum VkOrdContractRole
{
    /// <summary>
    /// Неизвестная роль
    /// </summary>
    [Description("Неизвестная роль")]
    [EnumMember(Value = "unknown")]
    Unknown = 0,

    /// <summary>
    /// Заказчик
    /// </summary>
    [Description("Заказчик")]
    [EnumMember(Value = "customer")]
    Customer = 1,

    /// <summary>
    /// Исполнитель
    /// </summary>
    [Description("Исполнитель")]
    [EnumMember(Value = "contractor")]
    Contractor = 2,
}