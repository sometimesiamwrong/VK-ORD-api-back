using Domain.Models.Requests;

namespace Domain.Handlers.Interfaces
{
    /// <summary>
    /// Обработчик запроса на создание контракта
    /// </summary>
    public interface ICreateContractHandler : ISimpleCommandHandler<CreateContractRequest>
    {
    }
}