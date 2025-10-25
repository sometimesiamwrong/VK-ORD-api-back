using System.ComponentModel;
using System.Runtime.Serialization;

namespace Domain.Entities.Enums
{
    /// <summary>
    /// Тип сущности
    /// </summary>
    public enum EntityType
    {
        /// <summary>
        /// Контрагент
        /// </summary>
        [EnumMember(Value = "counterparty")]
        [Description("Контрагент")]
        Counterparty = 1,

        /// <summary>
        /// Креатив
        /// </summary>
        [EnumMember(Value = "creative")]
        [Description("Креатив")]
        Creative = 2,

        /// <summary>
        /// Договор
        /// </summary>
        [EnumMember(Value = "contract")]
        [Description("Договор")]
        Contract = 3,
        
        /// <summary>
        /// Медиа
        /// </summary>
        [EnumMember(Value = "media")]
        [Description("Медиа")]
        Media = 4,

        /// <summary>
        /// Персона
        /// </summary>
        [EnumMember(Value = "person")]
        [Description("Персона")]
        Person = 5,

        /// <summary>
        /// Акт
        /// </summary>
        [EnumMember(Value = "invoice")]
        [Description("Акт")]
        Invoice = 6,

        /// <summary>
        /// Статистика
        /// </summary>
        [EnumMember(Value = "statistic")]
        [Description("Статистика")]
        Statistic = 7,

        /// <summary>
        /// Api учетные данные
        /// </summary>
        [EnumMember(Value = "api_credential")]
        [Description("Api учетные данные VK ORD API")]
        ApiCredential = 8,
    }
}