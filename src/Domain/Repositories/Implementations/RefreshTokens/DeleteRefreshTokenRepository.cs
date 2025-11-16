using Domain.Data;
using Domain.Repositories.Interfaces.RefreshTokens;

namespace Domain.Repositories.Implementations.RefreshTokens
{
    /// <summary>
    /// Репозиторий для удаления RefreshToken
    /// </summary>
    public class DeleteRefreshTokenRepository : IDeleteRefreshTokenRepository
    {
        private readonly Func<AppDbContext> _contextFactory;

        public DeleteRefreshTokenRepository(Func<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            await using var context = _contextFactory();
            var token = await context.RefreshTokens.FindAsync(id);
            if (token == null)
                return false;

            context.RefreshTokens.Remove(token);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
