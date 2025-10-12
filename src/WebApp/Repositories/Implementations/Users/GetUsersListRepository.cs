using Domain.Data;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using WebApp.Repositories.Interfaces.Users;

namespace WebApp.Repositories.Implementations.Users
{
    /// <summary>
    /// Репозиторий для получения списка пользователей
    /// </summary>
    public class GetUsersListRepository : IGetUsersListRepository
    {
        private readonly AppDbContext _db;

        public GetUsersListRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<User>> GetListAsync()
        {
            return await _db.Users.ToListAsync();
        }
    }
}
