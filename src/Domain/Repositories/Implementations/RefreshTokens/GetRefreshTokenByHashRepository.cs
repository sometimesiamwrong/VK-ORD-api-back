using Domain.Data;
using Domain.Entities;
using Domain.Repositories.Interfaces.RefreshTokens;
using Microsoft.EntityFrameworkCore;

namespace Domain.Repositories.Implementations.RefreshTokens
{
    /// <summary>
    /// Репозиторий для получения RefreshToken по хэшу токена
    /// </summary>
    public class GetRefreshTokenByHashRepository : IGetRefreshTokenByHashRepository
    {
        private readonly Func<AppDbContext> _contextFactory;

        public GetRefreshTokenByHashRepository(Func<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<RefreshToken?> GetByHashAsync(string tokenHash)
        {
            await using var context = _contextFactory();
            return await context.RefreshTokens
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.TokenHash == tokenHash && 
                                        r.RevokedAt == null && 
                                        r.ExpiresAt > DateTimeOffset.UtcNow);
        }
    }
}
