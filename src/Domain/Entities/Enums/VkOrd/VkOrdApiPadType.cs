using System.Runtime.Serialization;

namespace Domain.Entities.Enums.VkOrd;

public enum VkOrdApiPadType
{
    /// <summary>
    /// Веб-сайт
    /// </summary>
    [EnumMember(Value = "web")]
    Web,

    /// <summary>
    /// Мобильное приложение
    /// </summary>
    [EnumMember(Value = "mobile_app")]
    MobileApp,

    /// <summary>
    /// Социальная сеть
    /// </summary>
    [EnumMember(Value = "social_network")]
    SocialNetwork,

    /// <summary>
    /// Иное
    /// </summary>
    [EnumMember(Value = "other")]
    Other
}
