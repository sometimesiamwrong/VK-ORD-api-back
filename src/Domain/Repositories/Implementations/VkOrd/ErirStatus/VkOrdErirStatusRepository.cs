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

        // Используем PostgreSQL native UPSERT с bulk insert для максимальной производительности
        // Собираем все VALUES в один SQL запрос вместо множества отдельных запросов

        var parameters = new List<Npgsql.NpgsqlParameter>();
        var valuesClauses = new List<string>();

        for (int i = 0; i < statuses.Count; i++)
        {
            var status = statuses[i];
            var paramPrefix = $"p{i}_";

            valuesClauses.Add($@"
                (@{paramPrefix}publicId, @{paramPrefix}logicalAccountId, @{paramPrefix}externalId, @{paramPrefix}entityType, @{paramPrefix}erirStatus,
                 @{paramPrefix}updatedByUserTs, @{paramPrefix}finalizedTs, @{paramPrefix}errorMessages, @{paramPrefix}createdAt, @{paramPrefix}updatedAt, false)");

            parameters.Add(new Npgsql.NpgsqlParameter($"@{paramPrefix}publicId", Guid.NewGuid()));
            parameters.Add(new Npgsql.NpgsqlParameter($"@{paramPrefix}logicalAccountId", status.LogicalAccountId));
            parameters.Add(new Npgsql.NpgsqlParameter($"@{paramPrefix}externalId", status.ExternalId));
            parameters.Add(new Npgsql.NpgsqlParameter($"@{paramPrefix}entityType", (int)status.EntityType));
            parameters.Add(new Npgsql.NpgsqlParameter($"@{paramPrefix}erirStatus", (int)status.ErirStatus));
            parameters.Add(new Npgsql.NpgsqlParameter($"@{paramPrefix}updatedByUserTs", status.UpdatedByUserTs));
            parameters.Add(new Npgsql.NpgsqlParameter($"@{paramPrefix}finalizedTs", status.FinalizedTs ?? (object)DBNull.Value));
            parameters.Add(new Npgsql.NpgsqlParameter($"@{paramPrefix}errorMessages", status.ErrorMessages ?? (object)DBNull.Value));
            parameters.Add(new Npgsql.NpgsqlParameter($"@{paramPrefix}createdAt", now));
            parameters.Add(new Npgsql.NpgsqlParameter($"@{paramPrefix}updatedAt", now));
        }

        var sql = $@"
            INSERT INTO VkOrdErirStatuses
                (public_id, logical_account_id, external_id, entity_type, erir_status,
                 updated_by_user_ts, finalized_ts, error_messages, created_at, updated_at, is_deleted)
            VALUES
                {string.Join(",", valuesClauses)}
            ON CONFLICT (logical_account_id, entity_type, external_id)
            DO UPDATE SET
                erir_status = EXCLUDED.erir_status,
                updated_by_user_ts = EXCLUDED.updated_by_user_ts,
                finalized_ts = EXCLUDED.finalized_ts,
                error_messages = EXCLUDED.error_messages,
                updated_at = EXCLUDED.updated_at";

        await context.Database.ExecuteSqlRawAsync(sql, parameters, cancellationToken);
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
