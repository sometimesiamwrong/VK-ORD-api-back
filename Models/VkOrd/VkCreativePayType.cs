namespace VkOrdApiWrapper.Models.VkOrd
{
    /// <summary>
    /// Тип оплаты креатива VK ОРД
    /// </summary>
    public enum VkCreativePayType
    {
        /// <summary>
        /// Cost Per Action, цена за действие
        /// </summary>
        cpa = 1,

        /// <summary>
        /// Cost Per Click, цена за клик
        /// </summary>
        cpc = 2,

        /// <summary>
        /// Cost Per Millennium, цена за 1 000 показов
        /// </summary>
        cpm = 3,

        /// <summary>
        /// Иное
        /// </summary>
        other = 4
    }
}