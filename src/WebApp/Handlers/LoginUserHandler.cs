using Domain.BrokenRules;
using Domain.Extensions;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using WebApp.Handlers.Requests;
using WebApp.Repositories.Interfaces.Users;
using WebApp.Security;

namespace WebApp.Handlers
{
    public class LoginUserHandler : IRequestHandler<LoginUserQuery, TokenPair>
    {
        private readonly IGetUserByNameRepository _getUserByNameRepository;
        private readonly ITokenService _tokenService;
        private readonly IPasswordHasher<User> _hasher;

        public LoginUserHandler(
            IGetUserByNameRepository getUserByNameRepository,
            ITokenService tokenService)
        {
            _getUserByNameRepository = getUserByNameRepository;
            _tokenService = tokenService;
            _hasher = new PasswordHasher<User>();
        }

        public async Task<TokenPair> Handle(LoginUserQuery request, CancellationToken cancellationToken)
        {
            var user = await _getUserByNameRepository.GetByNameAsync(request.UserName);
            if (user == null || _hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password) == PasswordVerificationResult.Failed)
            {
                throw BrokenRuleCodes.InvalidCredentials.AsExn();
            }

            return await _tokenService.GenerateTokens(user, request.Ip);
        }
    }
}
