using Domain.Data;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using WebApp.Repositories.Interfaces.ApiCredentials;

namespace WebApp.Repositories.Implementations.ApiCredentials;

/// <summary>
/// Реализация репозитория для получения всех логических аккаунтов с credentials
/// </summary>
public class GetAllLogicalAccountsRepository : IGetAllLogicalAccountsRepository
{
    private readonly AppDbContext _context;

    public GetAllLogicalAccountsRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<(long LogicalAccountId, ApiCredential Credential)>> GetAllWithCredentials(
        CancellationToken cancellationToken)
    {
        // Получаем все уникальные LogicalAccountId и для каждого берем любой ApiCredential
        var logicalAccounts = await _context.ApiCredentials
            .Where(a => !a.IsDeleted)
            .GroupBy(a => a.LogicalAccountId)
            .Select(g => new
            {
                LogicalAccountId = g.Key,
                Credential = g.First()
            })
            .ToListAsync(cancellationToken);

        return logicalAccounts
            .Select(x => (x.LogicalAccountId, x.Credential))
            .ToList();
    }
}
