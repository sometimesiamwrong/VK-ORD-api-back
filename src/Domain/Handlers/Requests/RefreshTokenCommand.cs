using MediatR;
using WebApp.Security;

namespace Domain.Handlers.Requests
{
    public class RefreshTokenCommand : IRequest<TokenPair>
    {
        public required string RefreshToken { get; set; }
        public string? Ip { get; set; }
    }
}
