using System.Text.Json.Serialization;
using Domain.Models.Responses;

namespace Domain.Handlers.Requests
{
    /// <summary>
    /// Запрос на получение списка контрагентов
    /// </summary>
    public class GetCounterpartiesRequest : PageRequest, ICommand<GetCounterpartiesResponseDto>
    {
        /// <summary>
        /// Искать по внешнему ID
        /// </summary>
        [JsonPropertyName("external_id")]
        public string ExternalId { get; set; } = string.Empty;
    }
}
