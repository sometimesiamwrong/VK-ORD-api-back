using Domain.Data;
using WebApp.Repositories.Interfaces.RefreshTokens;

namespace WebApp.Repositories.Implementations.RefreshTokens
{
    /// <summary>
    /// Репозиторий для удаления RefreshToken
    /// </summary>
    public class DeleteRefreshTokenRepository : IDeleteRefreshTokenRepository
    {
        private readonly AppDbContext _db;

        public DeleteRefreshTokenRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var token = await _db.RefreshTokens.FindAsync(id);
            if (token == null)
                return false;

            _db.RefreshTokens.Remove(token);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
