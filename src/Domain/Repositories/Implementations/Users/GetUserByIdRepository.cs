using Domain.Data;
using Domain.Entities;
using Domain.Repositories.Interfaces.Users;
using Microsoft.EntityFrameworkCore;

namespace Domain.Repositories.Implementations.Users
{
    /// <summary>
    /// Репозиторий для получения пользователя по ID
    /// </summary>
    public class GetUserByIdRepository : IGetUserByIdRepository
    {
        private readonly Func<AppDbContext> _contextFactory;

        public GetUserByIdRepository(Func<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<User?> GetById(long id, CancellationToken cancellationToken)
        {
            await using var context = _contextFactory();
            return await context.Users.FindAsync(id, cancellationToken);
        }

        public async Task<User?> GetByGuid(Guid guid, CancellationToken cancellationToken)
        {
            await using var context = _contextFactory();
            return await context.Users.FirstOrDefaultAsync(u => u.PublicId == guid, cancellationToken);
        }
    }
}
