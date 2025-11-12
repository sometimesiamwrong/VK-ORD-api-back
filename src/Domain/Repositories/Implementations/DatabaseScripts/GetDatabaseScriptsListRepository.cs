using Domain.Data;
using Domain.Entities;
using Domain.Repositories.Interfaces.DatabaseScripts;
using Microsoft.EntityFrameworkCore;

namespace Domain.Repositories.Implementations.DatabaseScripts
{
    /// <summary>
    /// Репозиторий для получения списка DatabaseScripts
    /// </summary>
    public class GetDatabaseScriptsListRepository : IGetDatabaseScriptsListRepository
    {
        private readonly AppDbContext _db;

        public GetDatabaseScriptsListRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<DatabaseScript>> GetListAsync()
        {
            return await _db.DatabaseScripts.OrderByDescending(s => s.ExecutedAt).ToListAsync();
        }
    }
}
