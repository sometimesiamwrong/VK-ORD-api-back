using MediatR;

namespace WebApp.Handlers.Requests
{
    public class RevokeRefreshTokenCommand : IRequest<Unit>
    {
        public required string TokenHash { get; set; }
    }
}
