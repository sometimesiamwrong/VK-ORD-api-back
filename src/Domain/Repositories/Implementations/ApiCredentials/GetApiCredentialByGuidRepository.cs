using Domain.Data;
using Domain.Entities;
using Domain.Repositories.Interfaces.ApiCredentials;
using Microsoft.EntityFrameworkCore;

namespace Domain.Repositories.Implementations.ApiCredentials;

public class GetApiCredentialByGuidRepository : IGetApiCredentialByGuidRepository
{
    private readonly AppDbContext _db;

    public GetApiCredentialByGuidRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<ApiCredential?> GetByGuidAsync(Guid guid)
    {
        return _db.ApiCredentials.FirstOrDefaultAsync(c => c.PublicId == guid);
    }
}