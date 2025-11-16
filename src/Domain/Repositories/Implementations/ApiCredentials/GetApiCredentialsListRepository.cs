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
        private readonly Func<AppDbContext> _contextFactory;

        public GetApiCredentialsListRepository(Func<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<ApiCredential>> GetListAsync(long userId, CancellationToken cancellationToken, VkOrdApiEnvironmentCode? environment = null) 
        {
            await using var context = _contextFactory();
            var query = context.ApiCredentials.Where(c => c.UserId == userId);
            
            if (environment.HasValue)
            {
                query = query.Where(c => c.ApiEnvironment == environment.Value);
            }

            return await query.OrderByDescending(c => c.UpdatedAt).ToListAsync();
        }
    }
}
