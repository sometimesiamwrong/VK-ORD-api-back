using Domain.Data;
using Domain.Entities;
using Domain.Repositories.Interfaces.Users;
using Microsoft.EntityFrameworkCore;

namespace Domain.Repositories.Implementations.Users
{
    /// <summary>
    /// Репозиторий для получения списка пользователей
    /// </summary>
    public class GetUsersListRepository : IGetUsersListRepository
    {
        private readonly Func<AppDbContext> _contextFactory;

        public GetUsersListRepository(Func<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<User>> GetListAsync()
        {
            await using var context = _contextFactory();
            return await context.Users.ToListAsync();
        }
    }
}
