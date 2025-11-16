using Domain.Data;
using Domain.Entities;
using Domain.Repositories.Interfaces.ApiCredentials;
using Microsoft.EntityFrameworkCore;

namespace Domain.Repositories.Implementations.ApiCredentials
{
    /// <summary>
    /// Репозиторий для получения ApiCredential по ID
    /// </summary>
    public class GetApiCredentialByIdRepository : IGetApiCredentialByIdRepository
    {
        private readonly Func<AppDbContext> _contextFactory;

        public GetApiCredentialByIdRepository(Func<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<ApiCredential?> GetById(long id, CancellationToken cancellationToken)
        {
            await using var context = _contextFactory();
            return await context.ApiCredentials.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public async Task<ApiCredential?> GetByPublicId(Guid publicId, CancellationToken cancellationToken)
        {
            await using var context = _contextFactory();
            return await context.ApiCredentials.FirstOrDefaultAsync(c => c.PublicId == publicId, cancellationToken);
        }
    }
}
