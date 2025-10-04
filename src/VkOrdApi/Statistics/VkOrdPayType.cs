using System.Runtime.Serialization;

namespace VkOrdApi.Statistics;

public enum VkOrdPayType
{
    /// <summary>
    /// За тысячу показов (CPM)
    /// </summary>
    [EnumMember(Value = "cpm")]
    Cpm,

    /// <summary>
    /// За клик (CPC)
    /// </summary>
    [EnumMember(Value = "cpc")]
    Cpc,

    /// <summary>
    /// За действие (CPA)
    /// </summary>
    [EnumMember(Value = "cpa")]
    Cpa,

    /// <summary>
    /// Фиксированная (flat)
    /// </summary>
    [EnumMember(Value = "flat")]
    Flat
}
