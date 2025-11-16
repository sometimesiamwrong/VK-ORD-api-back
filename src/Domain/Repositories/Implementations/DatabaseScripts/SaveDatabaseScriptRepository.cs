using Domain.Data;
using Domain.Entities;
using Domain.Repositories.Interfaces.DatabaseScripts;

namespace Domain.Repositories.Implementations.DatabaseScripts
{
    /// <summary>
    /// Репозиторий для сохранения DatabaseScript
    /// </summary>
    public class SaveDatabaseScriptRepository : ISaveDatabaseScriptRepository
    {
        private readonly Func<AppDbContext> _contextFactory;

        public SaveDatabaseScriptRepository(Func<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<DatabaseScript?> SaveAsync(DatabaseScript script)
        {
            await using var context = _contextFactory();
            if (script.IsNewOrUpdate())
            {
                // Создание новой сущности
                context.DatabaseScripts.Add(script);
                await context.SaveChangesAsync();
                return script;
            }
            else
            {
                // Обновление существующей сущности
                var existing = await context.DatabaseScripts.FindAsync(script.Id);
                if (existing == null)
                    return null;

                existing.ScriptName = script.ScriptName;
                existing.ScriptHash = script.ScriptHash;
                existing.ExecutedAt = script.ExecutedAt;
                existing.Description = script.Description;
                existing.IsSuccessful = script.IsSuccessful;
                existing.ErrorMessage = script.ErrorMessage;

                await context.SaveChangesAsync();
                return existing;
            }
        }
    }
}
