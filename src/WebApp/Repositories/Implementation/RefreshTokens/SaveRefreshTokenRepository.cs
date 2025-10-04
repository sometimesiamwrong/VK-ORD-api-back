using Domain.Data;
using Domain.Entities;
using WebApp.Repositories.Interfaces.RefreshTokens;

namespace WebApp.Repositories.Implementation.RefreshTokens
{
    /// <summary>
    /// Репозиторий для сохранения RefreshToken
    /// </summary>
    public class SaveRefreshTokenRepository : ISaveRefreshTokenRepository
    {
        private readonly AppDbContext _db;

        public SaveRefreshTokenRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<RefreshToken?> SaveAsync(RefreshToken token)
        {
            if (token.IsNewOrUpdate())
            {
                // Создание новой сущности
                _db.RefreshTokens.Add(token);
                await _db.SaveChangesAsync();
                return token;
            }
            else
            {
                // Обновление существующей сущности
                var existing = await _db.RefreshTokens.FindAsync(token.Id);
                if (existing == null)
                    return null;

                existing.TokenHash = token.TokenHash;
                existing.ExpiresAt = token.ExpiresAt;
                existing.CreatedByIp = token.CreatedByIp;
                existing.DeviceId = token.DeviceId;
                existing.RevokedAt = token.RevokedAt;
                existing.ReplacedByTokenHash = token.ReplacedByTokenHash;
                existing.UpdatedAt = DateTimeOffset.UtcNow;

                await _db.SaveChangesAsync();
                return existing;
            }
        }
    }
}
