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
        private readonly AppDbContext _db;

        public GetRefreshTokensListRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<RefreshToken>> GetListAsync(long userId)
        {
            return await _db.RefreshTokens
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }
    }
}
