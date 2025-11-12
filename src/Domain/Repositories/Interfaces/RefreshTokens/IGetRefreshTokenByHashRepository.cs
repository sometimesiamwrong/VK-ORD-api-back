using Domain.Entities;

namespace Domain.Repositories.Interfaces.RefreshTokens
{
    /// <summary>
    /// Репозиторий для получения RefreshToken по хэшу токена
    /// </summary>
    public interface IGetRefreshTokenByHashRepository
    {
        /// <summary>
        /// Получить активный refresh token по хэшу с включением пользователя
        /// </summary>
        Task<RefreshToken?> GetByHashAsync(string tokenHash);
    }
}
