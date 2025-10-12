using System.Runtime.Serialization;

namespace Domain.Entities.Enums.VkOrd;

public enum VkOrdApiInvoiceStatus
{
    /// <summary>
    /// Черновик
    /// </summary>
    [EnumMember(Value = "draft")]
    Draft,

    /// <summary>
    /// Готов к отправке
    /// </summary>
    [EnumMember(Value = "ready")]
    Ready,

    /// <summary>
    /// Отправлено в ЕРИР
    /// </summary>
    [EnumMember(Value = "sent")]
    Sent,

    /// <summary>
    /// Одобрено
    /// </summary>
    [EnumMember(Value = "approved")]
    Approved,

    /// <summary>
    /// Удалено
    /// </summary>
    [EnumMember(Value = "deleted")]
    Deleted
}
