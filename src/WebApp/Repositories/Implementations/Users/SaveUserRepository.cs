using Domain.Data;
using Domain.Entities;
using WebApp.Repositories.Interfaces.Users;

namespace WebApp.Repositories.Implementations.Users
{
    /// <summary>
    /// Репозиторий для сохранения пользователя
    /// </summary>
    public class SaveUserRepository : ISaveUserRepository
    {
        private readonly AppDbContext _db;

        public SaveUserRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<User?> Save(User user, CancellationToken cancellationToken)
        {
            if (user.IsNewOrUpdate())
            {
                // Создание новой сущности
                _db.Users.Add(user);
                await _db.SaveChangesAsync(cancellationToken);
                return user;
            }
            else
            {
                // Обновление существующей сущности
                var existingUser = await _db.Users.FindAsync(user.Id, cancellationToken);
                if (existingUser == null)
                    return null;

                existingUser.UserName = user.UserName;
                existingUser.Name = user.Name;
                existingUser.PasswordHash = user.PasswordHash;
                existingUser.IsActive = user.IsActive;
                existingUser.UpdatedAt = DateTimeOffset.UtcNow;

                await _db.SaveChangesAsync(cancellationToken);
                return existingUser;
            }
        }
    }
}
