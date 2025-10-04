using Domain.Entities;

namespace WebApp.Repositories.Interfaces.Users
{
    /// <summary>
    /// Репозиторий для получения пользователя по ID
    /// </summary>
    public interface IGetUserByIdRepository
    {
        /// <summary>
        /// Получить пользователя по ID
        /// </summary>
        Task<User?> GetByIdAsync(long id);
    }
}
