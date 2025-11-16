using Domain.Data;
using Domain.Repositories.Interfaces.ApiCredentials;
using Microsoft.EntityFrameworkCore;

namespace Domain.Repositories.Implementations.ApiCredentials
{
    /// <summary>
    /// Репозиторий для удаления ApiCredential
    /// </summary>
    public class DeleteApiCredentialRepository : IDeleteApiCredentialRepository
    {
        private readonly Func<AppDbContext> _contextFactory;

        public DeleteApiCredentialRepository(Func<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<bool> Delete(Guid id, CancellationToken cancellationToken)
        {
            await using var context = _contextFactory();
            var credential = await context.ApiCredentials.FirstOrDefaultAsync(c => c.PublicId == id);
            if (credential == null)
                return false;

            context.ApiCredentials.Remove(credential);
            await context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
