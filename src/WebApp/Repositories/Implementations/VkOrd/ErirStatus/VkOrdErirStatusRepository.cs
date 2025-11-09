using Domain.Data;
using Domain.Entities.Enums;
using Domain.Entities.VkOrd;
using Microsoft.EntityFrameworkCore;
using WebApp.Repositories.Interfaces.VkOrd.ErirStatus;

namespace WebApp.Repositories.Implementations.VkOrd.ErirStatus;

/// <summary>
/// Реализация репозитория для работы с ERIR статусами сущностей VK ORD
/// </summary>
public class VkOrdErirStatusRepository : IVkOrdErirStatusRepository
{
    private readonly AppDbContext _context;

    public VkOrdErirStatusRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<VkOrdErirStatus?> GetByExternalId(
        long logicalAccountId,
        string externalId,
        EntityType entityType,
        CancellationToken cancellationToken)
    {
        return await _context.VkOrdErirStatuses
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
        return await _context.VkOrdErirStatuses
            .Where(e => e.LogicalAccountId == logicalAccountId
                && e.EntityType == entityType
                && !e.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task UpsertStatus(VkOrdErirStatus status, CancellationToken cancellationToken)
    {
        var existing = await GetByExternalId(
            status.LogicalAccountId,
            status.ExternalId,
            status.EntityType,
            cancellationToken);

        if (existing == null)
        {
            status.CreatedAt = DateTimeOffset.UtcNow;
            status.UpdatedAt = DateTimeOffset.UtcNow;
            status.PublicId = Guid.NewGuid();
            _context.VkOrdErirStatuses.Add(status);
        }
        else
        {
            existing.ErirStatus = status.ErirStatus;
            existing.UpdatedByUserTs = status.UpdatedByUserTs;
            existing.FinalizedTs = status.FinalizedTs;
            existing.ErrorMessages = status.ErrorMessages;
            existing.UpdatedAt = DateTimeOffset.UtcNow;
            _context.VkOrdErirStatuses.Update(existing);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<string>> GetAllExternalIds(
        long logicalAccountId,
        EntityType entityType,
        CancellationToken cancellationToken)
    {
        return await _context.VkOrdErirStatuses
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
        return entityType switch
        {
            EntityType.Counterparty => await _context.VkOrdCounterparties
                .Where(e => e.LogicalAccountId == logicalAccountId && !e.IsDeleted)
                .Select(e => e.ExternalId)
                .Distinct()
                .ToListAsync(cancellationToken),

            EntityType.Contract => await _context.VkOrdContracts
                .Where(e => e.LogicalAccountId == logicalAccountId && !e.IsDeleted)
                .Select(e => e.ExternalId)
                .Distinct()
                .ToListAsync(cancellationToken),

            EntityType.Creative => await _context.VkOrdCreatives
                .Where(e => e.LogicalAccountId == logicalAccountId && !e.IsDeleted)
                .Select(e => e.ExternalId)
                .Distinct()
                .ToListAsync(cancellationToken),

            EntityType.Invoice => await _context.VkOrdInvoices
                .Where(e => e.LogicalAccountId == logicalAccountId && !e.IsDeleted)
                .Select(e => e.ExternalId)
                .Distinct()
                .ToListAsync(cancellationToken),

            EntityType.Statistic => await _context.VkOrdStatistics
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
        return entityType switch
        {
            EntityType.Counterparty => await _context.VkOrdCounterparties
                .AnyAsync(e => e.LogicalAccountId == logicalAccountId
                    && e.ExternalId == externalId
                    && !e.IsDeleted, cancellationToken),

            EntityType.Contract => await _context.VkOrdContracts
                .AnyAsync(e => e.LogicalAccountId == logicalAccountId
                    && e.ExternalId == externalId
                    && !e.IsDeleted, cancellationToken),

            EntityType.Creative => await _context.VkOrdCreatives
                .AnyAsync(e => e.LogicalAccountId == logicalAccountId
                    && e.ExternalId == externalId
                    && !e.IsDeleted, cancellationToken),

            EntityType.Invoice => await _context.VkOrdInvoices
                .AnyAsync(e => e.LogicalAccountId == logicalAccountId
                    && e.ExternalId == externalId
                    && !e.IsDeleted, cancellationToken),

            EntityType.Statistic => await _context.VkOrdStatistics
                .AnyAsync(e => e.LogicalAccountId == logicalAccountId
                    && e.ExternalId == externalId
                    && !e.IsDeleted, cancellationToken),

            _ => throw new ArgumentException($"Unsupported entity type: {entityType}", nameof(entityType))
        };
    }
}
