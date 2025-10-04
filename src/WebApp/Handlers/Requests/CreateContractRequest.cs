using MediatR;
using WebApp.Models.Requests;
using WebApp.Models.Responses;

namespace WebApp.Handlers.Requests
{
    public record CreateContractRequestWrapper(CreateContractRequest Request, Guid UserId, string? Environment) : IRequest<CreateContractResponse>;
}
