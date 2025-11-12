using MediatR;
using WebApp.Security;

namespace Domain.Handlers.Requests
{
    public class LoginUserQuery : IRequest<TokenPair>
    {
        public required string UserName { get; set; }
        public required string Password { get; set; }
        public string? Ip { get; set; }
    }
}
