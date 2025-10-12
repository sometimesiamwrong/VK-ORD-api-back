using System.Runtime.Serialization;

namespace Domain.Entities.Enums.VkOrd;

/// <summary>
/// Статус обработки объекта в ЕРИР
/// </summary>
public enum VkOrdApiErirStatus
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
