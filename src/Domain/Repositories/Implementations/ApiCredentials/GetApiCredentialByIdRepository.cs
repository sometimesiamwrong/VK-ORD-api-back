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
        private readonly AppDbContext _db;

        public GetApiCredentialByIdRepository(AppDbContext db)
        {
            _db = db;
        }

        public Task<ApiCredential?> GetById(long id, CancellationToken cancellationToken)
        {
            return _db.ApiCredentials.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public Task<ApiCredential?> GetByPublicId(Guid publicId, CancellationToken cancellationToken)
        {
            return _db.ApiCredentials.FirstOrDefaultAsync(c => c.PublicId == publicId, cancellationToken);
        }
    }
}
