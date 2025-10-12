using System.Runtime.Serialization;

namespace Domain.Entities.Enums.VkOrd;

public enum VkOrdApiClientRole
{
    /// <summary>
    /// Рекламодатель
    /// </summary>
    [EnumMember(Value = "advertiser")]
    Advertiser,

    /// <summary>
    /// Рекламное агентство
    /// </summary>
    [EnumMember(Value = "agency")]
    Agency,

    /// <summary>
    /// Оператор рекламной системы
    /// </summary>
    [EnumMember(Value = "ors")]
    Ors,

    /// <summary>
    /// Издатель, рекламораспространитель
    /// </summary>
    [EnumMember(Value = "publisher")]
    Publisher,

    /// <summary>
    /// Посредник
    /// </summary>
    [EnumMember(Value = "mediator")]
    Mediator
}
