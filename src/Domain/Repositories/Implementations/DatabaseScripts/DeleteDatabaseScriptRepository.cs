using Domain.Data;
using Domain.Repositories.Interfaces.DatabaseScripts;

namespace Domain.Repositories.Implementations.DatabaseScripts
{
    /// <summary>
    /// Репозиторий для удаления DatabaseScript
    /// </summary>
    public class DeleteDatabaseScriptRepository : IDeleteDatabaseScriptRepository
    {
        private readonly Func<AppDbContext> _contextFactory;

        public DeleteDatabaseScriptRepository(Func<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            await using var context = _contextFactory();
            var script = await context.DatabaseScripts.FindAsync(id);
            if (script == null)
                return false;

            context.DatabaseScripts.Remove(script);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
