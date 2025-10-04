using Domain.Entities;

namespace WebApp.Repositories.Interfaces.RefreshTokens
{
    /// <summary>
    /// Репозиторий для получения RefreshToken по ID
    /// </summary>
    public interface IGetRefreshTokenByIdRepository
    {
        /// <summary>
        /// Получить RefreshToken по ID
        /// </summary>
        Task<RefreshToken?> GetByIdAsync(long id);
    }
}
