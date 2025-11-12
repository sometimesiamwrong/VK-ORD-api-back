using Domain.Handlers.Requests;
using Domain.Repositories.Interfaces.RefreshTokens;
using MediatR;

namespace Domain.Handlers
{
    public class RevokeRefreshTokenHandler : IRequestHandler<RevokeRefreshTokenCommand, Unit>
    {
        private readonly IGetRefreshTokenByHashRepository _getRefreshTokenByHashRepository;
        private readonly ISaveRefreshTokenRepository _saveRefreshTokenRepository;

        public RevokeRefreshTokenHandler(
            IGetRefreshTokenByHashRepository getRefreshTokenByHashRepository,
            ISaveRefreshTokenRepository saveRefreshTokenRepository)
        {
            _getRefreshTokenByHashRepository = getRefreshTokenByHashRepository;
            _saveRefreshTokenRepository = saveRefreshTokenRepository;
        }

        public async Task<Unit> Handle(RevokeRefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var token = await _getRefreshTokenByHashRepository.GetByHashAsync(request.TokenHash);
            if (token != null)
            {
                token.RevokedAt = DateTime.UtcNow;
                await _saveRefreshTokenRepository.SaveAsync(token);
            }

            return Unit.Value;
        }
    }
}
