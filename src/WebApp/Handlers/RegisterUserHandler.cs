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
    public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, TokenPair>
    {
        private readonly IGetUserByNameRepository _getUserByNameRepository;
        private readonly ISaveUserRepository _userSaveRepository;
        private readonly IPasswordHasher<User> _hasher;
        private readonly ITokenService _tokenService;

        public RegisterUserHandler(
            IGetUserByNameRepository getUserByNameRepository,
            ISaveUserRepository userSaveRepository,
            ITokenService tokenService)
        {
            _getUserByNameRepository = getUserByNameRepository;
            _userSaveRepository = userSaveRepository;
            _tokenService = tokenService;
            _hasher = new PasswordHasher<User>();
        }

        public async Task<TokenPair> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var exists = await _getUserByNameRepository.GetByName(request.UserName, cancellationToken);
            if (exists != null)
            {
                throw BrokenRuleCodes.UserWithSuchNameAlreadyExists.AsExn();
            }

            var user = new User
            {
                UserName = request.UserName,
                Name = request.Name,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            };

            user.PasswordHash = _hasher.HashPassword(user, request.Password);

            await _userSaveRepository.Save(user, cancellationToken);

            return await _tokenService.GenerateTokens(user, request.Ip);
        }
    }
}
