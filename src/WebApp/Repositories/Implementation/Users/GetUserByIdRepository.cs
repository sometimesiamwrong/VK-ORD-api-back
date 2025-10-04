using Domain.Data;
using Domain.Entities;
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

        public async Task<User?> GetByIdAsync(long id)
        {
            return await _db.Users.FindAsync(id);
        }
    }
}
