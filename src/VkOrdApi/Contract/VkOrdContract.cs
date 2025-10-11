using System.Text.Json.Serialization;
using VkOrdApi.Contract;

namespace VkOrdApi.Contract
{
    public class VkOrdContract
    {
        /// <summary>
        /// Внешний идентификатор договора.
        /// </summary>
        [JsonPropertyName("external_id")]
        public string ExternalId { get; set; } = string.Empty;

        /// <summary>
        /// Дата и время создания договора.
        /// </summary>
        [JsonPropertyName("create_date")]
        public string CreateDate { get; set; } = string.Empty;

        /// <summary>
        /// Тип договора. Возможные значения:
        /// service — договор оказания услуг.
        /// mediation — посреднический договор. Требует заполнения поля action_type.
        /// additional — дополнительное соглашение. Требует заполнения поля parent_contract_external_id.
        /// </summary>
        [JsonPropertyName("type")]
        public VkOrdContractType Type { get; set; }

        /// <summary>
        /// Внешний идентификатор клиента (заказчика).
        /// </summary>
        [JsonPropertyName("client_external_id")]
        public string ClientExternalId { get; set; } = string.Empty;

        /// <summary>
        /// Внешний идентификатор подрядчика (исполнителя).
        /// </summary>
        [JsonPropertyName("contractor_external_id")]
        public string ContractorExternalId { get; set; } = string.Empty;

        /// <summary>
        /// Осуществляемые действия (посредничество).
        /// </summary>
        [JsonPropertyName("action_type")]
        public VkOrdActionType? ActionType { get; set; }

        /// <summary>
        /// Предмет договора.
        /// </summary>
        [JsonPropertyName("subject_type")]
        public VkOrdSubjectType SubjectType { get; set; }

        /// <summary>
        /// Дата заключения договора.
        /// </summary>
        [JsonPropertyName("date")]
        public string? Date { get; set; }

        /// <summary>
        /// Дата окончания договора.
        /// </summary>
        [JsonPropertyName("date_end")]
        public string? DateEnd { get; set; }

        /// <summary>
        /// Серийный номер договора.
        /// </summary>
        [JsonPropertyName("serial")]
        public string? Serial { get; set; }

        /// <summary>
        /// Дополнительная информация о договоре.
        /// </summary>
        [JsonPropertyName("flags")]
        public List<VkOrdContractFlag> Flags { get; set; } = new();

        /// <summary>
        /// Внешний идентификатор родительского договора.
        /// </summary>
        [JsonPropertyName("parent_contract_external_id")]
        public string? ParentContractExternalId { get; set; }

        /// <summary>
        /// Цена договора.
        /// </summary>
        [JsonPropertyName("amount")]
        public string? Amount { get; set; }

        /// <summary>
        /// Признак наличия дополнительных соглашений.
        /// </summary>
        [JsonPropertyName("has_additional_contracts")]
        public bool HasAdditionalContracts { get; set; }

        /// <summary>
        /// CID контракта.
        /// </summary>
        [JsonPropertyName("cid")]
        public string? Cid { get; set; }

        /// <summary>
        /// Список заблокированных полей.
        /// </summary>
        [JsonPropertyName("locked_fields")]
        public List<VkOrdLockedField> LockedFields { get; set; } = new();
    }
}
