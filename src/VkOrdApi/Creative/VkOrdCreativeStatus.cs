using System.Runtime.Serialization;

namespace VkOrdApi.Creative;

public enum VkOrdCreativeStatus
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
