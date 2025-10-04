using Domain.Entities;

namespace WebApp.Repositories.Interfaces.Users
{
    /// <summary>
    /// Репозиторий для получения списка пользователей
    /// </summary>
    public interface IGetUsersListRepository
    {
        /// <summary>
        /// Получить список пользователей
        /// </summary>
        Task<List<User>> GetListAsync();
    }
}
