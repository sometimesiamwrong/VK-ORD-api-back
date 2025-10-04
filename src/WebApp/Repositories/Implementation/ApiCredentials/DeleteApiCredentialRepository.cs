using Domain.Data;
using WebApp.Repositories.Interfaces.ApiCredentials;

namespace WebApp.Repositories.Implementation.ApiCredentials
{
    /// <summary>
    /// Репозиторий для удаления ApiCredential
    /// </summary>
    public class DeleteApiCredentialRepository : IDeleteApiCredentialRepository
    {
        private readonly AppDbContext _db;

        public DeleteApiCredentialRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var credential = await _db.ApiCredentials.FindAsync(id);
            if (credential == null)
                return false;

            _db.ApiCredentials.Remove(credential);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
