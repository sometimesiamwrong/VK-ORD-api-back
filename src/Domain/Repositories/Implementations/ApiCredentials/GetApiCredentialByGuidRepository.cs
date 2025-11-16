using Domain.Data;
using Domain.Entities;
using Domain.Repositories.Interfaces.ApiCredentials;
using Microsoft.EntityFrameworkCore;

namespace Domain.Repositories.Implementations.ApiCredentials;

public class GetApiCredentialByGuidRepository : IGetApiCredentialByGuidRepository
{
    private readonly Func<AppDbContext> _contextFactory;

    public GetApiCredentialByGuidRepository(Func<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<ApiCredential?> GetByGuidAsync(Guid guid)
    {
        await using var context = _contextFactory();
        return await context.ApiCredentials.FirstOrDefaultAsync(c => c.PublicId == guid);
    }
}