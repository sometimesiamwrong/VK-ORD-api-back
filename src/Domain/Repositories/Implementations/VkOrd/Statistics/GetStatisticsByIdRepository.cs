using Domain.Data;
using Domain.Entities.VkOrd;
using Domain.Repositories.Interfaces.VkOrd.Statistics;
using Microsoft.EntityFrameworkCore;

namespace Domain.Repositories.Implementations.VkOrd.Statistics;

/// <summary>
/// Репозиторий для получения статистики по ID
/// Simplified: Statistics in VK ORD API work differently than other entities
/// </summary>
public class GetStatisticsByIdRepository : IGetStatisticsByIdRepository
{
    private readonly Func<AppDbContext> _contextFactory;
    private readonly ILogger<GetStatisticsByIdRepository> _logger;

    public GetStatisticsByIdRepository(
        Func<AppDbContext> contextFactory,
        ILogger<GetStatisticsByIdRepository> logger)
    {
        _contextFactory = contextFactory;
        _logger = logger;
    }

    public async Task<VkOrdStatistic> Get(string externalId, CancellationToken cancellationToken, bool noCache = false)
    {
        await using var context = _contextFactory();
        // For statistics, we just return from database
        // The ERIR sync job will handle refreshing if needed
        var data = await context.VkOrdStatistics
            .Include(x => x.Creative)
            .FirstOrDefaultAsync(x => x.ExternalId == externalId && !x.IsDeleted, cancellationToken);

        if (data == null)
        {
            throw new InvalidOperationException($"Statistic with external_id {externalId} not found");
        }

        return data;
    }
}
