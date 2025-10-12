using System.Runtime.Serialization;

namespace Domain.Entities.Enums.VkOrd;

public enum VkOrdApiCreativeFlag
{
    /// <summary>
    /// Социальная реклама
    /// </summary>
    [EnumMember(Value = "social")]
    Social,

    /// <summary>
    /// Нативная реклама
    /// </summary>
    /// <remarks>
    /// Только в GET, PUT не поддерживается
    /// </remarks>
    [EnumMember(Value = "native")]
    Native,

    /// <summary>
    /// Социальная реклама по квоте
    /// </summary>
    [EnumMember(Value = "social_quota")]
    SocialQuota,
}