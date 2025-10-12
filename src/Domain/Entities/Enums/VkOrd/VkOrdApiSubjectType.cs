using System.Runtime.Serialization;

namespace Domain.Entities.Enums.VkOrd;

public enum VkOrdApiSubjectType
{
    /// <summary>
    /// Представительство.
    /// </summary>
    [EnumMember(Value = "representation")]
    Representation,

    /// <summary>
    /// Организация распространения рекламы.
    /// </summary>
    [EnumMember(Value = "org_distribution")]
    OrgDistribution,

    /// <summary>
    /// Посредничество.
    /// </summary>
    [EnumMember(Value = "mediation")]
    Mediation,

    /// <summary>
    /// Распространение рекламы.
    /// </summary>
    [EnumMember(Value = "distribution")]
    Distribution,

    /// <summary>
    /// Иное.
    /// </summary>
    [EnumMember(Value = "other")]
    Other
}
