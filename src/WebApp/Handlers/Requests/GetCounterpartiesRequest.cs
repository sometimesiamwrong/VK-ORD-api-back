using MediatR;
using WebApp.Models.Requests;
using WebApp.Models.Responses;

namespace WebApp.Handlers.Requests
{
    /// <summary>
    /// Запрос на получение списка контрагентов
    /// </summary>
    public class GetCounterpartiesRequest : IRequestWithVkOrdKey, IRequest<GetCounterpartiesResponse>
    {
        /// <summary>
        /// Сдвиг
        /// </summary>
        public int? Offset { get; set; }

        /// <summary>
        /// Лимит
        /// </summary>
        public int? Limit { get; set; }
    }
}
