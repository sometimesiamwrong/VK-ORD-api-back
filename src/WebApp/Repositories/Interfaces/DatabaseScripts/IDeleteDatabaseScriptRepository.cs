namespace WebApp.Repositories.Interfaces.DatabaseScripts
{
    /// <summary>
    /// Репозиторий для удаления DatabaseScript
    /// </summary>
    public interface IDeleteDatabaseScriptRepository
    {
        /// <summary>
        /// Удалить DatabaseScript по ID
        /// </summary>
        Task<bool> DeleteAsync(long id);
    }
}
