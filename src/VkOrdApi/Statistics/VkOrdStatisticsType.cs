using System.Runtime.Serialization;

namespace VkOrdApi.Statistics;

public enum VkOrdStatisticsType
{
    /// <summary>
    /// Показы (impressions)
    /// </summary>
    [EnumMember(Value = "impressions")]
    Impressions,

    /// <summary>
    /// Клики
    /// </summary>
    [EnumMember(Value = "clicks")]
    Clicks,

    /// <summary>
    /// Просмотры (views)
    /// </summary>
    [EnumMember(Value = "views")]
    Views,

    /// <summary>
    /// Расходы (spends)
    /// </summary>
    [EnumMember(Value = "spends")]
    Spends
}
