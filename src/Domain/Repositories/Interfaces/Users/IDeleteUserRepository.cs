namespace Domain.Repositories.Interfaces.Users
{
    /// <summary>
    /// Репозиторий для удаления пользователя
    /// </summary>
    public interface IDeleteUserRepository
    {
        /// <summary>
        /// Удалить пользователя по ID
        /// </summary>
        Task<bool> DeleteAsync(long id);
    }
}
