using Domain.Data;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using WebApp.Repositories.Interfaces.ApiCredentials;

namespace WebApp.Repositories.Implementation.ApiCredentials;

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