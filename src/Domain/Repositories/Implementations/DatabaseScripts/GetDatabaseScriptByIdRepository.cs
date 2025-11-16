using Domain.Data;
using Domain.Entities;
using Domain.Repositories.Interfaces.DatabaseScripts;

namespace Domain.Repositories.Implementations.DatabaseScripts
{
    /// <summary>
    /// Репозиторий для получения DatabaseScript по ID
    /// </summary>
    public class GetDatabaseScriptByIdRepository : IGetDatabaseScriptByIdRepository
    {
        private readonly Func<AppDbContext> _contextFactory;

        public GetDatabaseScriptByIdRepository(Func<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<DatabaseScript?> GetByIdAsync(long id)
        {
            await using var context = _contextFactory();
            return await context.DatabaseScripts.FindAsync(id);
        }
    }
}
