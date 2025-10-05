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
        Task<User?> GetById(long id, CancellationToken cancellationToken);

        /// <summary>
        /// Получить пользователя по GUID
        /// </summary>
        Task<User?> GetByGuid(Guid guid, CancellationToken cancellationToken);
    }
}
