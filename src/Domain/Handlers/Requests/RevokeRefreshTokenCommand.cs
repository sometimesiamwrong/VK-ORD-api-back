using MediatR;

namespace Domain.Handlers.Requests
{
    public class RevokeRefreshTokenCommand : IRequest<Unit>
    {
        public required string TokenHash { get; set; }
    }
}
