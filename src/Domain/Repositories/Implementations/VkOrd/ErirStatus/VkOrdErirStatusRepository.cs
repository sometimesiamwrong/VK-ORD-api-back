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
            .Where(e => e.LogicalAccountId == logicalAccountId
                && e.EntityType == entityType
                && !e.IsDeleted)
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

        // Загружаем все существующие статусы одним запросом
        var externalIdsByEntity = statuses
            .GroupBy(s => new { s.LogicalAccountId, s.EntityType })
            .ToList();

        var existingStatuses = new Dictionary<string, VkOrdErirStatus>();

        foreach (var group in externalIdsByEntity)
        {
            var externalIds = group.Select(s => s.ExternalId).ToList();
            var existing = await context.VkOrdErirStatuses
                .Where(s => s.LogicalAccountId == group.Key.LogicalAccountId
                    && s.EntityType == group.Key.EntityType
                    && externalIds.Contains(s.ExternalId))
                .ToListAsync(cancellationToken);

            foreach (var status in existing)
            {
                var key = $"{status.LogicalAccountId}_{status.EntityType}_{status.ExternalId}";
                existingStatuses[key] = status;
            }
        }

        // Обрабатываем все статусы
        var newStatuses = new List<VkOrdErirStatus>();
        var now = DateTimeOffset.UtcNow;

        foreach (var status in statuses)
        {
            var key = $"{status.LogicalAccountId}_{status.EntityType}_{status.ExternalId}";

            if (existingStatuses.TryGetValue(key, out var existing))
            {
                // Обновляем существующий
                existing.ErirStatus = status.ErirStatus;
                existing.UpdatedByUserTs = status.UpdatedByUserTs;
                existing.FinalizedTs = status.FinalizedTs;
                existing.ErrorMessages = status.ErrorMessages;
                existing.UpdatedAt = now;
            }
            else
            {
                // Добавляем новый
                status.CreatedAt = now;
                status.UpdatedAt = now;
                status.PublicId = Guid.NewGuid();
                newStatuses.Add(status);
            }
        }

        if (newStatuses.Count > 0)
        {
            context.VkOrdErirStatuses.AddRange(newStatuses);
        }

        // Одно сохранение для всех изменений
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
                .Where(e => e.LogicalAccountId == logicalAccountId && !e.IsDeleted)
                .Select(e => e.ExternalId)
                .Distinct()
                .ToListAsync(cancellationToken),

            EntityType.Contract => await context.VkOrdContracts
                .Where(e => e.LogicalAccountId == logicalAccountId && !e.IsDeleted)
                .Select(e => e.ExternalId)
                .Distinct()
                .ToListAsync(cancellationToken),

            EntityType.Creative => await context.VkOrdCreatives
                .Where(e => e.LogicalAccountId == logicalAccountId && !e.IsDeleted)
                .Select(e => e.ExternalId)
                .Distinct()
                .ToListAsync(cancellationToken),

            EntityType.Invoice => await context.VkOrdInvoices
                .Where(e => e.LogicalAccountId == logicalAccountId && !e.IsDeleted)
                .Select(e => e.ExternalId)
                .Distinct()
                .ToListAsync(cancellationToken),

            EntityType.Statistic => await context.VkOrdStatistics
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
                .AnyAsync(e => e.LogicalAccountId == logicalAccountId
                    && e.ExternalId == externalId
                    && !e.IsDeleted, cancellationToken),

            EntityType.Contract => await context.VkOrdContracts
                .AnyAsync(e => e.LogicalAccountId == logicalAccountId
                    && e.ExternalId == externalId
                    && !e.IsDeleted, cancellationToken),

            EntityType.Creative => await context.VkOrdCreatives
                .AnyAsync(e => e.LogicalAccountId == logicalAccountId
                    && e.ExternalId == externalId
                    && !e.IsDeleted, cancellationToken),

            EntityType.Invoice => await context.VkOrdInvoices
                .AnyAsync(e => e.LogicalAccountId == logicalAccountId
                    && e.ExternalId == externalId
                    && !e.IsDeleted, cancellationToken),

            EntityType.Statistic => await context.VkOrdStatistics
                .AnyAsync(e => e.LogicalAccountId == logicalAccountId
                    && e.ExternalId == externalId
                    && !e.IsDeleted, cancellationToken),

            _ => throw new ArgumentException($"Unsupported entity type: {entityType}", nameof(entityType))
        };
    }
}
