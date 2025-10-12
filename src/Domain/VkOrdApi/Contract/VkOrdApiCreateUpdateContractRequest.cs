using System.Text.Json.Serialization;
using Domain.Entities.Enums.VkOrd;

namespace Domain.VkOrdApi.Contract;

public sealed class VkOrdApiCreateUpdateContractRequest
{
    /// <summary>
    /// Тип договора. Возможные значения:
    /// service — договор оказания услуг.
    /// mediation — посреднический договор. Требует заполнения поля action_type.
    /// additional — дополнительное соглашение. Требует заполнения поля parent_contract_external_id.
    /// </summary>
    [JsonPropertyName("type")]
    public VkOrdApiContractType Type { get; set; }

    /// <summary>
    /// Внешний идентификатор клиента (заказчика).
    /// Поля client_external_id и contractor_external_id должны принимать разные значения.
    /// Поля juridical_details.inn контрагентов (для иностранных контрагентов - juridical_details.foreign_inn)
    /// должны принимать разные значения, если заданы.
    /// Если поле type принимает значение additional, поля client_external_id и contractor_external_id
    /// должны принимать те же значения, что и в договоре, к которому создаётся соглашение.
    /// </summary>
    [JsonPropertyName("client_external_id")]
    public string ClientExternalId { get; set; } = string.Empty;

    /// <summary>
    /// Внешний идентификатор подрядчика (исполнителя).
    /// Поля client_external_id и contractor_external_id должны принимать разные значения.
    /// Поля juridical_details.inn контрагентов (для иностранных контрагентов - juridical_details.foreign_inn)
    /// должны принимать разные значения, если заданы.
    /// Если поле type принимает значение additional, поля client_external_id и contractor_external_id
    /// должны принимать те же значения, что и в договоре, к которому создаётся соглашение.
    /// </summary>
    [JsonPropertyName("contractor_external_id")]
    public string ContractorExternalId { get; set; } = string.Empty;

    /// <summary>
    /// Дата заключения договора в формате YYYY-MM-DD без привязки к часовому поясу.
    /// Минимальное значение — 1991-01-01, максимальное — дата запроса
    /// (дата запроса определяется на основе текущего времени в таймзоне UTC).
    /// </summary>
    [JsonPropertyName("date")]
    public string? Date { get; set; }

    /// <summary>
    /// Дата окончания договора в формате YYYY-MM-DD без привязки к часовому поясу.
    /// Минимальное значение — дата заключения договора.
    /// </summary>
    [JsonPropertyName("date_end")]
    public string? DateEnd { get; set; }

    /// <summary>
    /// Серийный номер договора. Может отсутствовать.
    /// Максимальная длина — 255 символов.
    /// При заполнении поля учитывайте рекомендации.
    /// </summary>
    [JsonPropertyName("serial")]
    public string? Serial { get; set; }

    /// <summary>
    /// Осуществляемые действия (посредничество).
    /// Поле обязательно и допустимо только в том случае, когда поле type принимает значение mediation.
    /// Возможные значения:
    /// distribution — распространение рекламы.
    /// conclude — заключение договоров.
    /// commercial — коммерческое представительство.
    /// other — иное.
    /// </summary>
    [JsonPropertyName("action_type")]
    public VkOrdApiActionType? ActionType { get; set; }

    /// <summary>
    /// Предмет договора. Возможные значения:
    /// representation — представительство.
    /// org_distribution — организация распространения рекламы.
    /// mediation — посредничество.
    /// distribution — распространение рекламы.
    /// other — иное.
    /// </summary>
    [JsonPropertyName("subject_type")]
    public VkOrdApiSubjectType ApiSubjectType { get; set; }

    /// <summary>
    /// Дополнительная информация о договоре. Возможные значения:
    /// vat_included — все налоги (если есть) включены в сумму договора.
    /// Обязателен для значений amount > 0.
    /// contractor_is_creatives_reporter — подрядчик обязуется вести учёт креативов.
    /// Значение можно указать, только если contractor_external_id — рекламная система.
    /// agent_acting_for_publisher — деньги поступают от подрядчика (исполнителя) клиенту (заказчику).
    /// Значение можно указать, только если поле type принимает значение mediation.
    /// is_charge_paid_by_agent — рекламный сбор в размере 3% за всю цепочку распространения рекламы
    /// оплачивает исполнитель по этому договору.
    /// Значение можно указать, только если поле type принимает значение mediation.
    /// Несовместим с флагом agent_acting_for_publisher.
    /// </summary>
    [JsonPropertyName("flags")]
    public List<VkOrdApiContractFlag> Flags { get; set; } = new();

    /// <summary>
    /// Внешний идентификатор родительского договора, к которому текущий договор будет
    /// являться допсоглашением.
    /// Поле обязательно и допустимо только в том случае, когда поле type принимает значение additional.
    /// Невозможно создать допсоглашение к допсоглашению.
    /// </summary>
    [JsonPropertyName("parent_contract_external_id")]
    public string? ParentContractExternalId { get; set; }

    /// <summary>
    /// Цена договора в рублях с копейками.
    /// Не может быть нулевой для типа договора mediation и для типа additional,
    /// родительский договор которого имеет тип mediation.
    /// Значение сохраняется с переданной точностью, но отправляется в ЕРИР и валидируется
    /// с округлением до 2 знаков после запятой по правилам математики.
    /// </summary>
    [JsonPropertyName("amount")]
    public string? Amount { get; set; }
}
