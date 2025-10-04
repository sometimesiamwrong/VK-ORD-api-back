using System.Runtime.Serialization;

namespace VkOrdApi.Dict;

/// <summary>
/// Язык описания для словарей (ККТУ и переводы ошибок ЕРИР).
/// </summary>
public enum VkOrdDictLang
{
    /// <summary>
    /// Русский язык (по умолчанию).
    /// </summary>
    [EnumMember(Value = "ru")]
    Ru,

    /// <summary>
    /// Английский язык.
    /// </summary>
    [EnumMember(Value = "en")]
    En,

    /// <summary>
    /// Китайский язык (с fallback на en, затем ru).
    /// </summary>
    [EnumMember(Value = "cn")]
    Cn
}
