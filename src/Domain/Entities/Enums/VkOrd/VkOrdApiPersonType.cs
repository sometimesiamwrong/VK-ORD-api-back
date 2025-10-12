using System.Runtime.Serialization;

namespace Domain.Entities.Enums.VkOrd
{
    /// <summary>
    /// Тип контрагента
    /// </summary>
    public enum VkOrdApiPersonType
    {
        /// <summary>
        /// Неизвестный тип
        /// </summary>
        [EnumMember(Value = "undefined")]
        Undefined = 0,

        /// <summary>
        /// Физическое лицо
        /// </summary>
        [EnumMember(Value = "physical")]
        Physical = 1,

        /// <summary>
        /// Юридическое лицо
        /// </summary>
        [EnumMember(Value = "juridical")]
        Juridical = 2,

        /// <summary>
        /// Индивидуальный предприниматель
        /// </summary>
        [EnumMember(Value = "ip")]
        Ip = 3,

        /// <summary>
        /// Иностранное физическое лицо
        /// </summary>
        [EnumMember(Value = "foreign_physical")]
        ForeignPhysical = 4,

        /// <summary>
        /// Иностранное юридическое лицо
        /// </summary>
        [EnumMember(Value = "foreign_juridical")]
        ForeignJuridical = 5,
    }
}