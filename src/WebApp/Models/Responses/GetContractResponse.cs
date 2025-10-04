namespace WebApp.Models.Responses
{
    /// <summary>
    /// Ответ при получении информации о контракте
    /// </summary>
    public class ContractResponse
    {
        /// <summary>
        /// Данные контракта
        /// </summary>
        public VkOrdContract Data { get; set; } = new();

        /// <summary>
        /// Внешний ID контракта
        /// </summary>
        public string ExternalId { get; set; } = string.Empty;
    }
}
