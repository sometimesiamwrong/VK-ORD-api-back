using Domain.Data;
using Domain.Entities;
using Domain.Repositories.Interfaces.RefreshTokens;
using Microsoft.EntityFrameworkCore;

namespace Domain.Repositories.Implementations.RefreshTokens
{
    /// <summary>
    /// Репозиторий для получения списка RefreshTokens
    /// </summary>
    public class GetRefreshTokensListRepository : IGetRefreshTokensListRepository
    {
        private readonly Func<AppDbContext> _contextFactory;

        public GetRefreshTokensListRepository(Func<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<RefreshToken>> GetListAsync(long userId)
        {
            await using var context = _contextFactory();
            return await context.RefreshTokens
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }
    }
}
