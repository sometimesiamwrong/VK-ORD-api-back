using Domain.Data;
using Domain.Entities;
using Domain.Repositories.Interfaces.Users;

namespace Domain.Repositories.Implementations.Users
{
    /// <summary>
    /// Репозиторий для сохранения пользователя
    /// </summary>
    public class SaveUserRepository : ISaveUserRepository
    {
        private readonly Func<AppDbContext> _contextFactory;

        public SaveUserRepository(Func<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<User?> Save(User user, CancellationToken cancellationToken)
        {
            await using var context = _contextFactory();
            if (user.IsNewOrUpdate())
            {
                // Создание новой сущности
                context.Users.Add(user);
                await context.SaveChangesAsync(cancellationToken);
                return user;
            }
            else
            {
                // Обновление существующей сущности
                var existingUser = await context.Users.FindAsync(user.Id, cancellationToken);
                if (existingUser == null)
                    return null;

                existingUser.UserName = user.UserName;
                existingUser.Name = user.Name;
                existingUser.PasswordHash = user.PasswordHash;
                existingUser.IsActive = user.IsActive;
                existingUser.UpdatedAt = DateTimeOffset.UtcNow;

                await context.SaveChangesAsync(cancellationToken);
                return existingUser;
            }
        }
    }
}
