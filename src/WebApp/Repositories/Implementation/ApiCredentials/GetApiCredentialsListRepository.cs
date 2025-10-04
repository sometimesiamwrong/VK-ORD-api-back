using Domain.Data;
using Domain.Entities;
using Domain.Entities.Enums;
using Microsoft.EntityFrameworkCore;
using WebApp.Repositories.Interfaces.ApiCredentials;

namespace WebApp.Repositories.Implementation.ApiCredentials
{
    /// <summary>
    /// Репозиторий для получения списка ApiCredentials
    /// </summary>
    public class GetApiCredentialsListRepository : IGetApiCredentialsListRepository
    {
        private readonly AppDbContext _db;

        public GetApiCredentialsListRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<ApiCredential>> GetListAsync(long userId, VkOrdEnvironmentCode? environment = null)
        {
            var query = _db.ApiCredentials.Where(c => c.UserId == userId);
            
            if (environment.HasValue)
            {
                query = query.Where(c => c.Environment == environment.Value);
            }

            return await query.OrderByDescending(c => c.UpdatedAt).ToListAsync();
        }
    }
}
