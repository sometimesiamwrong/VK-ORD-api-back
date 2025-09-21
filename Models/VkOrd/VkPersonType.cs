namespace VkOrdApiWrapper.Models.VkOrd
{
    public enum VkPersonType
    {
        /// <summary>
        /// Неизвестный тип
        /// </summary>
        undefined = 0,

        /// <summary>
        /// Физическое лицо
        /// </summary>  
        physical = 1,
        /// <summary>
        /// Юридическое лицо
        /// </summary>
        juridical = 2,

        /// <summary>
        /// Индивидуальный предприниматель
        /// </summary>
        ip = 3,

        /// <summary>
        /// Иностранное физическое лицо
        /// </summary>
        foreign_physical = 4,

        /// <summary>
        /// Иностранное юридическое лицо
        /// </summary>
        foreign_juridical = 5,
    }
}