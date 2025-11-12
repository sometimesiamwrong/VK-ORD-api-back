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
