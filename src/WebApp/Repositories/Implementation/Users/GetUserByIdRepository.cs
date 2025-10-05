using Domain.Data;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using WebApp.Repositories.Interfaces.Users;

namespace WebApp.Repositories.Implementation.Users
{
    /// <summary>
    /// Репозиторий для получения пользователя по ID
    /// </summary>
    public class GetUserByIdRepository : IGetUserByIdRepository
    {
        private readonly AppDbContext _db;

        public GetUserByIdRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<User?> GetById(long id, CancellationToken cancellationToken)
        {
            return await _db.Users.FindAsync(id, cancellationToken);
        }

        public async Task<User?> GetByGuid(Guid guid, CancellationToken cancellationToken)
        {
            return await _db.Users.FirstOrDefaultAsync(u => u.PublicId == guid, cancellationToken);
        }
    }
}
