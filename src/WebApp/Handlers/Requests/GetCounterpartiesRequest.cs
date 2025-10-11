using Domain;
using MediatR;
using WebApp.Models.Requests;
using WebApp.Models.Responses;

namespace WebApp.Handlers.Requests
{
    /// <summary>
    /// Запрос на получение списка контрагентов
    /// </summary>
    public class GetCounterpartiesRequest : ICommand<GetCounterpartiesResponseDto>
    {
        public PageRequest PageRequest { get; set; }
    }
}
