using Domain.Data;
using Domain.Entities;
using Domain.Repositories.Interfaces.RefreshTokens;

namespace Domain.Repositories.Implementations.RefreshTokens
{
    /// <summary>
    /// Репозиторий для сохранения RefreshToken
    /// </summary>
    public class SaveRefreshTokenRepository : ISaveRefreshTokenRepository
    {
        private readonly Func<AppDbContext> _contextFactory;

        public SaveRefreshTokenRepository(Func<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<RefreshToken?> SaveAsync(RefreshToken token)
        {
            await using var context = _contextFactory();
            if (token.IsNewOrUpdate())
            {
                // Создание новой сущности
                context.RefreshTokens.Add(token);
                await context.SaveChangesAsync();
                return token;
            }
            else
            {
                // Обновление существующей сущности
                var existing = await context.RefreshTokens.FindAsync(token.Id);
                if (existing == null)
                    return null;

                existing.TokenHash = token.TokenHash;
                existing.ExpiresAt = token.ExpiresAt;
                existing.CreatedByIp = token.CreatedByIp;
                existing.DeviceId = token.DeviceId;
                existing.RevokedAt = token.RevokedAt;
                existing.ReplacedByTokenHash = token.ReplacedByTokenHash;
                existing.UpdatedAt = DateTimeOffset.UtcNow;

                await context.SaveChangesAsync();
                return existing;
            }
        }
    }
}
