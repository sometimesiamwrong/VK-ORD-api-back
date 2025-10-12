using System.Runtime.Serialization;

namespace Domain.Entities.Enums.VkOrd;

public enum VkOrdApiContractType
{
    /// <summary>
    /// Договор оказания услуг.
    /// </summary>
    [EnumMember(Value = "service")]
    Service,

    /// <summary>
    /// Посреднический договор. Требует заполнения поля action_type.
    /// </summary>
    [EnumMember(Value = "mediation")]
    Mediation,

    /// <summary>
    /// Дополнительное соглашение. Требует заполнения поля parent_contract_external_id.
    /// </summary>
    [EnumMember(Value = "additional")]
    Additional
}
