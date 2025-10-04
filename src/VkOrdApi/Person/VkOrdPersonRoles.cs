using System.Runtime.Serialization;

namespace VkOrdApi.Person;

/// <summary>
/// Роли контрагента в VK ОРД.
/// </summary>
public enum VkOrdPersonRoles
{
    /// <summary>
    /// Рекламодатель.
    /// </summary>
    [EnumMember(Value = "advertiser")]
    Advertiser,

    /// <summary>
    /// Рекламное агентство.
    /// </summary>
    [EnumMember(Value = "agency")]
    Agency,

    /// <summary>
    /// Оператор рекламной системы.
    /// </summary>
    [EnumMember(Value = "ors")]
    Ors,

    /// <summary>
    /// Издатель, рекламораспространитель.
    /// </summary>
    [EnumMember(Value = "publisher")]
    Publisher
}
