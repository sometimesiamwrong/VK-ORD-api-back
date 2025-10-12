using System.Runtime.Serialization;

namespace Domain.Entities.Enums.VkOrd;

public enum VkOrdApiActionType
{
    /// <summary>
    /// Распространение рекламы.
    /// </summary>
    [EnumMember(Value = "distribution")]
    Distribution,

    /// <summary>
    /// Заключение договоров.
    /// </summary>
    [EnumMember(Value = "conclude")]
    Conclude,

    /// <summary>
    /// Коммерческое представительство.
    /// </summary>
    [EnumMember(Value = "commercial")]
    Commercial,

    /// <summary>
    /// Иное.
    /// </summary>
    [EnumMember(Value = "other")]
    Other
}
