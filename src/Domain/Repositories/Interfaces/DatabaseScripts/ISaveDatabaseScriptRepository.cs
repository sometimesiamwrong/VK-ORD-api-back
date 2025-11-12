using Domain.Entities;

namespace Domain.Repositories.Interfaces.DatabaseScripts
{
    /// <summary>
    /// Репозиторий для сохранения DatabaseScript
    /// </summary>
    public interface ISaveDatabaseScriptRepository
    {
        /// <summary>
        /// Сохранить DatabaseScript (создать или обновить)
        /// </summary>
        Task<DatabaseScript?> SaveAsync(DatabaseScript script);
    }
}
