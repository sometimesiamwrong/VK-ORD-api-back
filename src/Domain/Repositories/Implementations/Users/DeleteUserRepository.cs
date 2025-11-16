using Domain.Data;
using Domain.Repositories.Interfaces.Users;

namespace Domain.Repositories.Implementations.Users
{
    /// <summary>
    /// Репозиторий для удаления пользователя
    /// </summary>
    public class DeleteUserRepository : IDeleteUserRepository
    {
        private readonly Func<AppDbContext> _contextFactory;

        public DeleteUserRepository(Func<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            await using var context = _contextFactory();
            var user = await context.Users.FindAsync(id);
            if (user == null)
                return false;

            context.Users.Remove(user);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
