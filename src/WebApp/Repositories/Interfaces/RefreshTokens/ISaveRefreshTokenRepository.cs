using Domain.Entities;

namespace WebApp.Repositories.Interfaces.RefreshTokens
{
    /// <summary>
    /// Репозиторий для сохранения RefreshToken
    /// </summary>
    public interface ISaveRefreshTokenRepository
    {
        /// <summary>
        /// Сохранить RefreshToken (создать или обновить)
        /// </summary>
        Task<RefreshToken?> SaveAsync(RefreshToken token);
    }
}
