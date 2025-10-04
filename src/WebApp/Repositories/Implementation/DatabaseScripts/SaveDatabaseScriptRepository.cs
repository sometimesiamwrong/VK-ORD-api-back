using Domain.Data;
using Domain.Entities;
using WebApp.Repositories.Interfaces.DatabaseScripts;

namespace WebApp.Repositories.Implementation.DatabaseScripts
{
    /// <summary>
    /// Репозиторий для сохранения DatabaseScript
    /// </summary>
    public class SaveDatabaseScriptRepository : ISaveDatabaseScriptRepository
    {
        private readonly AppDbContext _db;

        public SaveDatabaseScriptRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<DatabaseScript?> SaveAsync(DatabaseScript script)
        {
            if (script.IsNewOrUpdate())
            {
                // Создание новой сущности
                _db.DatabaseScripts.Add(script);
                await _db.SaveChangesAsync();
                return script;
            }
            else
            {
                // Обновление существующей сущности
                var existing = await _db.DatabaseScripts.FindAsync(script.Id);
                if (existing == null)
                    return null;

                existing.ScriptName = script.ScriptName;
                existing.ScriptHash = script.ScriptHash;
                existing.ExecutedAt = script.ExecutedAt;
                existing.Description = script.Description;
                existing.IsSuccessful = script.IsSuccessful;
                existing.ErrorMessage = script.ErrorMessage;
                existing.UpdatedAt = DateTimeOffset.UtcNow;

                await _db.SaveChangesAsync();
                return existing;
            }
        }
    }
}
