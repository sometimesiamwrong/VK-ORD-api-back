using MediatR;
using WebApp.Models.Requests;
using WebApp.Models.Responses;

namespace WebApp.Handlers.Requests
{
    public record CreateCreativeRequestWrapper(CreateCreativeRequest Request, Guid UserId, string? Environment) : IRequest<CreateCreativeResponse>;
}
