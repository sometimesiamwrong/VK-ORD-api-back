using Domain.Data;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using WebApp.Repositories.Interfaces.RefreshTokens;

namespace WebApp.Repositories.Implementation.RefreshTokens
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
