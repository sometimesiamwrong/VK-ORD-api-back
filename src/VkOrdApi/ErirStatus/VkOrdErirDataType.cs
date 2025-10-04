using System.Runtime.Serialization;

namespace VkOrdApi.ErirStatus;

/// <summary>
/// Типы объектов по работе с рекламой для фильтрации статусов ЕРИР.
/// </summary>
public enum VkOrdErirDataType
{
    /// <summary>
    /// Контрагенты.
    /// </summary>
    [EnumMember(Value = "person")]
    Person,

    /// <summary>
    /// Договоры.
    /// </summary>
    [EnumMember(Value = "contract")]
    Contract,

    /// <summary>
    /// Креативы.
    /// </summary>
    [EnumMember(Value = "creative")]
    Creative,

    /// <summary>
    /// Рекламные площадки.
    /// </summary>
    [EnumMember(Value = "pad")]
    Pad,

    /// <summary>
    /// Акты.
    /// </summary>
    [EnumMember(Value = "invoice")]
    Invoice,

    /// <summary>
    /// Статистика.
    /// </summary>
    [EnumMember(Value = "statistics")]
    Statistics,

    /// <summary>
    /// Уникальный идентификатор изначального договора (CID - Contract ID).
    /// </summary>
    [EnumMember(Value = "cid")]
    Cid
}
