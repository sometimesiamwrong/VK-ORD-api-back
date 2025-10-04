using System.Runtime.Serialization;

namespace VkOrdApi.ErirStatus;

/// <summary>
/// Статус обработки объекта в ЕРИР
/// </summary>
public enum VkOrdErirStatus
{
    /// <summary>
    /// В обработке на стороне ОРД VK или ЕРИР
    /// </summary>
    [EnumMember(Value = "processing")]
    Processing,

    /// <summary>
    /// Не прошёл проверку ОРД VK или ЕРИР
    /// </summary>
    [EnumMember(Value = "bad")]
    Bad,

    /// <summary>
    /// Проверка ЕРИР пройдена успешно
    /// </summary>
    [EnumMember(Value = "verified")]
    Verified
}
