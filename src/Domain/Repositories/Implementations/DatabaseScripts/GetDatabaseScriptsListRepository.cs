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
        private readonly Func<AppDbContext> _contextFactory;

        public GetDatabaseScriptsListRepository(Func<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<DatabaseScript>> GetListAsync()
        {
            await using var context = _contextFactory();
            return await context.DatabaseScripts.OrderByDescending(s => s.ExecutedAt).ToListAsync();
        }
    }
}
