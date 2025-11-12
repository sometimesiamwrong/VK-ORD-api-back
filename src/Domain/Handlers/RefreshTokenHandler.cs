using Domain.BrokenRules;
using Domain.Extensions;
using Domain.Handlers.Requests;
using Domain.Repositories.Interfaces.RefreshTokens;
using MediatR;
using WebApp.Security;

namespace Domain.Handlers
{
    public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, TokenPair>
    {
        private readonly IGetRefreshTokenByHashRepository _getRefreshTokenByHashRepository;
        private readonly ISaveRefreshTokenRepository _saveRefreshTokenRepository;
        private readonly ITokenService _tokenService;

        public RefreshTokenHandler(
            IGetRefreshTokenByHashRepository getRefreshTokenByHashRepository,
            ISaveRefreshTokenRepository saveRefreshTokenRepository,
            ITokenService tokenService)
        {
            _getRefreshTokenByHashRepository = getRefreshTokenByHashRepository;
            _saveRefreshTokenRepository = saveRefreshTokenRepository;
            _tokenService = tokenService;
        }

        public async Task<TokenPair> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var refreshHash = TokenService.ComputeSha256(request.RefreshToken);
            var entity = await _getRefreshTokenByHashRepository.GetByHashAsync(refreshHash);
            if (entity == null)
            {
                throw BrokenRuleCodes.UserNotAuthorized.AsExn();
            }

            // rotate
            entity.RevokedAt = DateTime.UtcNow;
            var newTokens = await _tokenService.GenerateTokens(entity.User, request.Ip);
            entity.ReplacedByTokenHash = TokenService.ComputeSha256(newTokens.RefreshToken);
            await _saveRefreshTokenRepository.SaveAsync(entity);

            return newTokens;
        }
    }
}
