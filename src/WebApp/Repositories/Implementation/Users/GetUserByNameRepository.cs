using Domain.Data;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using WebApp.Repositories.Interfaces.Users;

namespace WebApp.Repositories.Implementation.Users
{
    /// <summary>
    /// Репозиторий для получения пользователя по имени
    /// </summary>
    public class GetUserByNameRepository : IGetUserByNameRepository
    {
        private readonly AppDbContext _db;

        public GetUserByNameRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<User?> GetByName(string username, CancellationToken cancellationToken)
        {
            return await _db.Users.FirstOrDefaultAsync(u => u.UserName == username, cancellationToken);
        }
    }
}
