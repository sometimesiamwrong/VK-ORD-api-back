using Domain.Data;
using Domain.Entities;
using Domain.Repositories.Interfaces.Users;
using Microsoft.EntityFrameworkCore;

namespace Domain.Repositories.Implementations.Users
{
    /// <summary>
    /// Репозиторий для получения пользователя по имени
    /// </summary>
    public class GetUserByNameRepository : IGetUserByNameRepository
    {
        private readonly Func<AppDbContext> _contextFactory;

        public GetUserByNameRepository(Func<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<User?> GetByName(string username, CancellationToken cancellationToken)
        {
            await using var context = _contextFactory();
            return await context.Users.FirstOrDefaultAsync(u => u.UserName == username, cancellationToken);
        }
    }
}
