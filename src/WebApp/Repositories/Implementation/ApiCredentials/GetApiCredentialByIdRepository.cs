using Domain.Data;
using Domain.Entities;
using WebApp.Repositories.Interfaces.ApiCredentials;

namespace WebApp.Repositories.Implementation.ApiCredentials
{
    /// <summary>
    /// Репозиторий для получения ApiCredential по ID
    /// </summary>
    public class GetApiCredentialByIdRepository : IGetApiCredentialByIdRepository
    {
        private readonly AppDbContext _db;

        public GetApiCredentialByIdRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<ApiCredential?> GetByIdAsync(long id)
        {
            return await _db.ApiCredentials.FindAsync(id);
        }
    }
}
