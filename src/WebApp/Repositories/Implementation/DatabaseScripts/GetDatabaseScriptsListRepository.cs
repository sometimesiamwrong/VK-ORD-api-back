using Domain.Data;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using WebApp.Repositories.Interfaces.DatabaseScripts;

namespace WebApp.Repositories.Implementation.DatabaseScripts
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
