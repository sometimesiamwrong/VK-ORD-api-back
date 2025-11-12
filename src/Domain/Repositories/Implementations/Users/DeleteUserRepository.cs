using Domain.Data;
using Domain.Repositories.Interfaces.Users;

namespace Domain.Repositories.Implementations.Users
{
    /// <summary>
    /// Репозиторий для удаления пользователя
    /// </summary>
    public class DeleteUserRepository : IDeleteUserRepository
    {
        private readonly AppDbContext _db;

        public DeleteUserRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null)
                return false;

            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
