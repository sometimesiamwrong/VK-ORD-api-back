using System.Runtime.Serialization;

namespace VkOrdApi.Statistics;

public enum VkOrdPayType
{
    /// <summary>
    /// Cost Per Millennium, цена за 1 000 показов.
    /// </summary>
    [EnumMember(Value = "cpm")]
    Cpm,

    /// <summary>
    /// Cost Per Click, цена за клик.
    /// </summary>
    [EnumMember(Value = "cpc")]
    Cpc,

    /// <summary>
    /// Cost Per Action, цена за действие.
    /// </summary>
    [EnumMember(Value = "cpa")]
    Cpa,

    /// <summary>
    /// Иное
    /// </summary>
    [EnumMember(Value = "other")]
    Other,
}