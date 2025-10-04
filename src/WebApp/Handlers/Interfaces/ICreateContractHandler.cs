using MediatR;
using WebApp.Handlers.Requests;
using WebApp.Models.Responses;

namespace WebApp.Handlers.Interfaces
{
    /// <summary>
    /// Обработчик запроса на создание контракта
    /// </summary>
    public interface ICreateContractHandler : IRequestHandler<CreateContractRequestWrapper, CreateContractResponse>
    {
    }
}