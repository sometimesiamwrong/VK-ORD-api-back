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
    private readonly AppDbContext _context;
    private readonly ILogger<GetStatisticsByIdRepository> _logger;

    public GetStatisticsByIdRepository(
        AppDbContext context,
        ILogger<GetStatisticsByIdRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<VkOrdStatistic> Get(string externalId, CancellationToken cancellationToken, bool noCache = false)
    {
        // For statistics, we just return from database
        // The ERIR sync job will handle refreshing if needed
        var data = await _context.VkOrdStatistics
            .Include(x => x.Creative)
            .FirstOrDefaultAsync(x => x.ExternalId == externalId && !x.IsDeleted, cancellationToken);

        if (data == null)
        {
            throw new InvalidOperationException($"Statistic with external_id {externalId} not found");
        }

        return data;
    }
}
