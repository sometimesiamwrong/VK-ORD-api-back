using Domain.Data;
using Domain.Entities;
using WebApp.Repositories.Interfaces.DatabaseScripts;

namespace WebApp.Repositories.Implementations.DatabaseScripts
{
    /// <summary>
    /// Репозиторий для получения DatabaseScript по ID
    /// </summary>
    public class GetDatabaseScriptByIdRepository : IGetDatabaseScriptByIdRepository
    {
        private readonly AppDbContext _db;

        public GetDatabaseScriptByIdRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<DatabaseScript?> GetByIdAsync(long id)
        {
            return await _db.DatabaseScripts.FindAsync(id);
        }
    }
}
