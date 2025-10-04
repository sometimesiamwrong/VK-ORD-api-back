using Domain.Entities;

namespace WebApp.Repositories.Interfaces.Users
{
    /// <summary>
    /// Репозиторий для получения пользователя по имени
    /// </summary>
    public interface IGetUserByNameRepository
    {
        /// <summary>
        /// Получить пользователя по имени
        /// </summary>
        Task<User?> GetByNameAsync(string name);
    }
}
