using Domain.Entities;

namespace Domain.Repositories.Interfaces.RefreshTokens
{
    /// <summary>
    /// Репозиторий для получения списка RefreshTokens
    /// </summary>
    public interface IGetRefreshTokensListRepository
    {
        /// <summary>
        /// Получить список RefreshTokens для пользователя
        /// </summary>
        Task<List<RefreshToken>> GetListAsync(long userId);
    }
}
