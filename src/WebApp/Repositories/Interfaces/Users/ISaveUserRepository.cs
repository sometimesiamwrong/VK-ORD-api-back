using Domain.Entities;

namespace WebApp.Repositories.Interfaces.Users
{
    /// <summary>
    /// Репозиторий для сохранения пользователя
    /// </summary>
    public interface ISaveUserRepository
    {
        /// <summary>
        /// Сохранить пользователя (создать или обновить)
        /// </summary>
        Task<User?> SaveAsync(User user);
    }
}
