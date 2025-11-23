using Domain.Data;
using Domain.Entities.Enums;
using Domain.Entities.VkOrd;
using Domain.Repositories.Interfaces.VkOrd.ErirStatus;
using Microsoft.EntityFrameworkCore;

namespace Domain.Repositories.Implementations.VkOrd.ErirStatus;

/// <summary>
/// Реализация репозитория для работы с ERIR статусами сущностей VK ORD
/// </summary>
public class VkOrdErirStatusRepository : IVkOrdErirStatusRepository
{
    private readonly Func<AppDbContext> _contextFactory;

    public VkOrdErirStatusRepository(Func<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<VkOrdErirStatus?> GetByExternalId(
        long logicalAccountId,
        string externalId,
        EntityType entityType,
        CancellationToken cancellationToken)
    {
        await using var context = _contextFactory();
        return await context.VkOrdErirStatuses
            .AsNoTracking()
            .Where(e => e.LogicalAccountId == logicalAccountId
                && e.ExternalId == externalId
                && e.EntityType == entityType
                && !e.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<VkOrdErirStatus>> GetAllByLogicalAccount(
        long logicalAccountId,
        EntityType entityType,
        CancellationToken cancellationToken)
    {
        await using var context = _contextFactory();
        return await context.VkOrdErirStatuses
            .AsNoTracking()
            .Where(e => e.LogicalAccountId == logicalAccountId
                && e.EntityType == entityType
                && !e.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<VkOrdErirStatus>> GetAllByLogicalAccount(
        long logicalAccountId,
        CancellationToken cancellationToken)
    {
        await using var context = _contextFactory();
        return await context.VkOrdErirStatuses
            .AsNoTracking()
            .Where(e => e.LogicalAccountId == logicalAccountId && !e.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task UpsertStatus(VkOrdErirStatus status, CancellationToken cancellationToken)
    {
        await using var context = _contextFactory();
        var existing = await GetByExternalIdInternal(
            context,
            status.LogicalAccountId,
            status.ExternalId,
            status.EntityType,
            cancellationToken);

        if (existing == null)
        {
            status.CreatedAt = DateTimeOffset.UtcNow;
            status.UpdatedAt = DateTimeOffset.UtcNow;
            status.PublicId = Guid.NewGuid();
            context.VkOrdErirStatuses.Add(status);
        }
        else
        {
            existing.ErirStatus = status.ErirStatus;
            existing.UpdatedByUserTs = status.UpdatedByUserTs;
            existing.FinalizedTs = status.FinalizedTs;
            existing.ErrorMessages = status.ErrorMessages;
            existing.UpdatedAt = DateTimeOffset.UtcNow;
            context.VkOrdErirStatuses.Update(existing);
        }

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpsertStatusesBatch(List<VkOrdErirStatus> statuses, CancellationToken cancellationToken)
    {
        if (statuses == null || statuses.Count == 0)
            return;

        await using var context = _contextFactory();
        var now = DateTimeOffset.UtcNow;

        var logicalAccountIds = statuses.Select(s => s.LogicalAccountId).Distinct().ToList();
        var entityTypes = statuses.Select(s => s.EntityType).Distinct().ToList();

        var existingStatuses = await context.VkOrdErirStatuses
            .Where(e => logicalAccountIds.Contains(e.LogicalAccountId)
                && entityTypes.Contains(e.EntityType)
                && !e.IsDeleted)
            .ToListAsync(cancellationToken);

        var externalIds = statuses.Select(s => s.ExternalId).Distinct().ToHashSet();
        var filteredExisting = existingStatuses
            .Where(e => externalIds.Contains(e.ExternalId))
            .ToList();

        var existingLookup = filteredExisting
            .ToDictionary(e => (e.LogicalAccountId, e.EntityType, e.ExternalId));

        foreach (var status in statuses)
        {
            var key = (status.LogicalAccountId, status.EntityType, status.ExternalId);

            if (existingLookup.TryGetValue(key, out var existing))
            {
                existing.ErirStatus = status.ErirStatus;
                existing.UpdatedByUserTs = status.UpdatedByUserTs;
                existing.FinalizedTs = status.FinalizedTs;
                existing.ErrorMessages = status.ErrorMessages;
                existing.UpdatedAt = now;
                context.VkOrdErirStatuses.Update(existing);
            }
            else
            {
                status.PublicId = Guid.NewGuid();
                status.CreatedAt = now;
                status.UpdatedAt = now;
                context.VkOrdErirStatuses.Add(status);
                existingLookup[key] = status;
            }
        }

        await context.SaveChangesAsync(cancellationToken);
    }

    private async Task<VkOrdErirStatus?> GetByExternalIdInternal(
        AppDbContext context,
        long logicalAccountId,
        string externalId,
        EntityType entityType,
        CancellationToken cancellationToken)
    {
        return await context.VkOrdErirStatuses
            .AsNoTracking()
            .Where(e => e.LogicalAccountId == logicalAccountId
                && e.ExternalId == externalId
                && e.EntityType == entityType
                && !e.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<string>> GetAllExternalIds(
        long logicalAccountId,
        EntityType entityType,
        CancellationToken cancellationToken)
    {
        await using var context = _contextFactory();
        return await context.VkOrdErirStatuses
            .AsNoTracking()
            .Where(e => e.LogicalAccountId == logicalAccountId
                && e.EntityType == entityType
                && !e.IsDeleted)
            .Select(e => e.ExternalId)
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    public async Task<List<string>> GetAllExternalIdsFromVkOrdEntities(
        long logicalAccountId,
        EntityType entityType,
        CancellationToken cancellationToken)
    {
        await using var context = _contextFactory();
        return entityType switch
        {
            EntityType.Counterparty => await context.VkOrdCounterparties
                .AsNoTracking()
                .Where(e => e.LogicalAccountId == logicalAccountId && !e.IsDeleted)
                .Select(e => e.ExternalId)
                .Distinct()
                .ToListAsync(cancellationToken),

            EntityType.Contract => await context.VkOrdContracts
                .AsNoTracking()
                .Where(e => e.LogicalAccountId == logicalAccountId && !e.IsDeleted)
                .Select(e => e.ExternalId)
                .Distinct()
                .ToListAsync(cancellationToken),

            EntityType.Creative => await context.VkOrdCreatives
                .AsNoTracking()
                .Where(e => e.LogicalAccountId == logicalAccountId && !e.IsDeleted)
                .Select(e => e.ExternalId)
                .Distinct()
                .ToListAsync(cancellationToken),

            EntityType.Invoice => await context.VkOrdInvoices
                .AsNoTracking()
                .Where(e => e.LogicalAccountId == logicalAccountId && !e.IsDeleted)
                .Select(e => e.ExternalId)
                .Distinct()
                .ToListAsync(cancellationToken),

            EntityType.Statistic => await context.VkOrdStatistics
                .AsNoTracking()
                .Where(e => e.LogicalAccountId == logicalAccountId && !e.IsDeleted)
                .Select(e => e.ExternalId)
                .Distinct()
                .ToListAsync(cancellationToken),

            _ => throw new ArgumentException($"Unsupported entity type: {entityType}", nameof(entityType))
        };
    }

    public async Task<bool> EntityExists(
        long logicalAccountId,
        string externalId,
        EntityType entityType,
        CancellationToken cancellationToken)
    {
        await using var context = _contextFactory();
        return entityType switch
        {
            EntityType.Counterparty => await context.VkOrdCounterparties
                .AsNoTracking()
                .AnyAsync(e => e.LogicalAccountId == logicalAccountId
                    && e.ExternalId == externalId
                    && !e.IsDeleted, cancellationToken),

            EntityType.Contract => await context.VkOrdContracts
                .AsNoTracking()
                .AnyAsync(e => e.LogicalAccountId == logicalAccountId
                    && e.ExternalId == externalId
                    && !e.IsDeleted, cancellationToken),

            EntityType.Creative => await context.VkOrdCreatives
                .AsNoTracking()
                .AnyAsync(e => e.LogicalAccountId == logicalAccountId
                    && e.ExternalId == externalId
                    && !e.IsDeleted, cancellationToken),

            EntityType.Invoice => await context.VkOrdInvoices
                .AsNoTracking()
                .AnyAsync(e => e.LogicalAccountId == logicalAccountId
                    && e.ExternalId == externalId
                    && !e.IsDeleted, cancellationToken),

            EntityType.Statistic => await context.VkOrdStatistics
                .AsNoTracking()
                .AnyAsync(e => e.LogicalAccountId == logicalAccountId
                    && e.ExternalId == externalId
                    && !e.IsDeleted, cancellationToken),

            _ => throw new ArgumentException($"Unsupported entity type: {entityType}", nameof(entityType))
        };
    }
}
