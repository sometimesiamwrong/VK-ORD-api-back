namespace VkOrdApiWrapper.Models.VkOrd
{
    public enum VkContactFlags
    {
        /// <summary>
        /// Неизвестный флаг
        /// </summary>
        undefined = 0,

        /// <summary>
        /// Все налоги (если есть) включены в сумму договора. Обязателен для значений amount > 0.
        /// </summary>
        vat_included = 1,

        /// <summary>
        /// Подрядчик обязуется вести учёт креативов. Значение можно указать, только если contractor_external_id — рекламная система.
        /// </summary>
        contractor_is_creatives_reporter = 2,

        /// <summary>
        /// Деньги поступают от подрядчика (исполнителя) клиенту (заказчику). Значение можно указать, только если поле type принимает значение mediation.
        /// </summary>
        agent_acting_for_publisher = 3,

        /// <summary>
        /// Рекламный сбор в размере 3% за всю цепочку распространения рекламы оплачивает исполнитель по этому договору. Значение можно указать, только если поле type принимает значение mediation. Несовместим с флагом agent_acting_for_publisher.
        /// </summary>
        is_charge_paid_by_agent = 4
    }
}