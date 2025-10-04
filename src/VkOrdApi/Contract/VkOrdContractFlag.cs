using System.Runtime.Serialization;

namespace VkOrdApi.Contract;

public enum VkOrdContractFlag
{
    /// <summary>
    /// Все налоги (если есть) включены в сумму договора.
    /// Обязателен для значений amount > 0.
    /// </summary>
    [EnumMember(Value = "vat_included")]
    VatIncluded,

    /// <summary>
    /// Подрядчик обязуется вести учёт креативов.
    /// Значение можно указать, только если contractor_external_id — рекламная система.
    /// </summary>
    [EnumMember(Value = "contractor_is_creatives_reporter")]
    ContractorIsCreativesReporter,

    /// <summary>
    /// Деньги поступают от подрядчика (исполнителя) клиенту (заказчику).
    /// Значение можно указать, только если поле type принимает значение mediation.
    /// </summary>
    [EnumMember(Value = "agent_acting_for_publisher")]
    AgentActingForPublisher,

    /// <summary>
    /// Рекламный сбор в размере 3% за всю цепочку распространения рекламы
    /// оплачивает исполнитель по этому договору.
    /// Значение можно указать, только если поле type принимает значение mediation.
    /// Несовместим с флагом agent_acting_for_publisher.
    /// </summary>
    [EnumMember(Value = "is_charge_paid_by_agent")]
    IsChargePaidByAgent
}
