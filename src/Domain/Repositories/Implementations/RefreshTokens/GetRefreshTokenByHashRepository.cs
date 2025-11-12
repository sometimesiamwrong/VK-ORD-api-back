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
        private readonly AppDbContext _db;

        public GetRefreshTokenByHashRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<RefreshToken?> GetByHashAsync(string tokenHash)
        {
            return await _db.RefreshTokens
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.TokenHash == tokenHash && 
                                        r.RevokedAt == null && 
                                        r.ExpiresAt > DateTimeOffset.UtcNow);
        }
    }
}
