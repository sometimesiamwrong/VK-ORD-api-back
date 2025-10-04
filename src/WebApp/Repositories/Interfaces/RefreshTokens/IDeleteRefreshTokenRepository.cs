namespace WebApp.Repositories.Interfaces.RefreshTokens
{
    /// <summary>
    /// Репозиторий для удаления RefreshToken
    /// </summary>
    public interface IDeleteRefreshTokenRepository
    {
        /// <summary>
        /// Удалить RefreshToken по ID
        /// </summary>
        Task<bool> DeleteAsync(long id);
    }
}
