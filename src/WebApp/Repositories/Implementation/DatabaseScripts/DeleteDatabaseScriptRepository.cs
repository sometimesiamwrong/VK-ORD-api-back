using Domain.Data;
using WebApp.Repositories.Interfaces.DatabaseScripts;

namespace WebApp.Repositories.Implementation.DatabaseScripts
{
    /// <summary>
    /// Репозиторий для удаления DatabaseScript
    /// </summary>
    public class DeleteDatabaseScriptRepository : IDeleteDatabaseScriptRepository
    {
        private readonly AppDbContext _db;

        public DeleteDatabaseScriptRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var script = await _db.DatabaseScripts.FindAsync(id);
            if (script == null)
                return false;

            _db.DatabaseScripts.Remove(script);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
