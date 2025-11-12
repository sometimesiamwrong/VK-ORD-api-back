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
