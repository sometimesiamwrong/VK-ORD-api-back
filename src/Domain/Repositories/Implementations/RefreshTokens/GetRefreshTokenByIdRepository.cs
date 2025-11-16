using Domain.Data;
using Domain.Entities;
using Domain.Repositories.Interfaces.RefreshTokens;

namespace Domain.Repositories.Implementations.RefreshTokens
{
    /// <summary>
    /// Репозиторий для получения RefreshToken по ID
    /// </summary>
    public class GetRefreshTokenByIdRepository : IGetRefreshTokenByIdRepository
    {
        private readonly Func<AppDbContext> _contextFactory;

        public GetRefreshTokenByIdRepository(Func<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<RefreshToken?> GetByIdAsync(long id)
        {
            await using var context = _contextFactory();
            return await context.RefreshTokens.FindAsync(id);
        }
    }
}
