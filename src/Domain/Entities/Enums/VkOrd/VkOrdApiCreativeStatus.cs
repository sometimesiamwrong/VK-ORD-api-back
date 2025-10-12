using System.Runtime.Serialization;

namespace Domain.Entities.Enums.VkOrd;

public enum VkOrdApiCreativeStatus
{
    /// <summary>
    /// Черновик
    /// </summary>
    [EnumMember(Value = "draft")]
    Draft,

    /// <summary>
    /// Отправлено на модерацию
    /// </summary>
    [EnumMember(Value = "submitted")]
    Submitted,

    /// <summary>
    /// Одобрено
    /// </summary>
    [EnumMember(Value = "approved")]
    Approved,

    /// <summary>
    /// Отклонено
    /// </summary>
    [EnumMember(Value = "rejected")]
    Rejected
}
