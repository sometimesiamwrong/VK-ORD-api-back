using Domain.Data;
using Domain.Entities;
using Domain.Entities.Enums.VkOrd;
using Domain.Repositories.Interfaces.ApiCredentials;
using Microsoft.EntityFrameworkCore;

namespace Domain.Repositories.Implementations.ApiCredentials
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

        public async Task<List<ApiCredential>> GetListAsync(long userId, CancellationToken cancellationToken, VkOrdApiEnvironmentCode? environment = null) 
        {
            var query = _db.ApiCredentials.Where(c => c.UserId == userId);
            
            if (environment.HasValue)
            {
                query = query.Where(c => c.ApiEnvironment == environment.Value);
            }

            return await query.OrderByDescending(c => c.UpdatedAt).ToListAsync();
        }
    }
}
