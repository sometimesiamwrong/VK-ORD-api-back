using Domain.Entities;

namespace WebApp.Repositories.Interfaces.DatabaseScripts
{
    /// <summary>
    /// Репозиторий для получения списка DatabaseScripts
    /// </summary>
    public interface IGetDatabaseScriptsListRepository
    {
        /// <summary>
        /// Получить список DatabaseScripts
        /// </summary>
        Task<List<DatabaseScript>> GetListAsync();
    }
}
