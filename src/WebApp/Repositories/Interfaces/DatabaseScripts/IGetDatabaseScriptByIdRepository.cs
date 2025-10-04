using Domain.Entities;

namespace WebApp.Repositories.Interfaces.DatabaseScripts
{
    /// <summary>
    /// Репозиторий для получения DatabaseScript по ID
    /// </summary>
    public interface IGetDatabaseScriptByIdRepository
    {
        /// <summary>
        /// Получить DatabaseScript по ID
        /// </summary>
        Task<DatabaseScript?> GetByIdAsync(long id);
    }
}
