using Domain.Data;
using Domain.Entities;
using Domain.Repositories.Interfaces.ApiCredentials;
using Microsoft.EntityFrameworkCore;

namespace Domain.Repositories.Implementations.ApiCredentials;

/// <summary>
/// Реализация репозитория для получения всех логических аккаунтов с credentials
/// </summary>
public class GetAllLogicalAccountsRepository : IGetAllLogicalAccountsRepository
{
    private readonly Func<AppDbContext> _contextFactory;

    public GetAllLogicalAccountsRepository(Func<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<(long LogicalAccountId, ApiCredential Credential)>> GetAllWithCredentials(
        CancellationToken cancellationToken)
    {
        await using var context = _contextFactory();
        // Получаем все уникальные LogicalAccountId и для каждого берем любой ApiCredential
        var logicalAccounts = await context.ApiCredentials
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
