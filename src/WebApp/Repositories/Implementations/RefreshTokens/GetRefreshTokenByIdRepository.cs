using Domain.Data;
using Domain.Entities;
using WebApp.Repositories.Interfaces.RefreshTokens;

namespace WebApp.Repositories.Implementations.RefreshTokens
{
    /// <summary>
    /// Репозиторий для получения RefreshToken по ID
    /// </summary>
    public class GetRefreshTokenByIdRepository : IGetRefreshTokenByIdRepository
    {
        private readonly AppDbContext _db;

        public GetRefreshTokenByIdRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<RefreshToken?> GetByIdAsync(long id)
        {
            return await _db.RefreshTokens.FindAsync(id);
        }
    }
}
