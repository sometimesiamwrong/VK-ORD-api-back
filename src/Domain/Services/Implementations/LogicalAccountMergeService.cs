using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain.Data;
using Domain.Entities;
using Domain.Entities.VkOrd;
using Domain.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Domain.Services.Implementations;

public class LogicalAccountMergeService : ILogicalAccountMergeService
{
    private readonly Func<AppDbContext> _contextFactory;
    private readonly ILogger<LogicalAccountMergeService> _logger;

    public LogicalAccountMergeService(
        Func<AppDbContext> contextFactory,
        ILogger<LogicalAccountMergeService> logger)
    {
        _contextFactory = contextFactory;
        _logger = logger;
    }

    public async Task<long?> FindAccountWithAllCreatives(
        List<string> externalIds,
        long currentAccountId,
        CancellationToken cancellationToken = default)
    {
        if (externalIds == null || externalIds.Count == 0)
        {
            _logger.LogDebug("No external IDs provided for account matching");
            return null;
        }

        await using var context = _contextFactory();

        // Find accounts that contain ALL specified external_id values
        var accountsWithAllCreatives = await context.VkOrdCreatives
            .Where(c => externalIds.Contains(c.ExternalId) &&
                       c.LogicalAccountId != currentAccountId &&
                       !c.IsDeleted)
            .GroupBy(c => c.LogicalAccountId)
            .Where(g => g.Count() == externalIds.Count)
            .Select(g => g.Key)
            .FirstOrDefaultAsync(cancellationToken);

        if (accountsWithAllCreatives != 0)
        {
            // Verify that the found account doesn't have additional creatives
            var foundAccountCreativeCount = await context.VkOrdCreatives
                .Where(c => c.LogicalAccountId == accountsWithAllCreatives && !c.IsDeleted)
                .CountAsync(cancellationToken);

            if (foundAccountCreativeCount == externalIds.Count)
            {
                _logger.LogInformation(
                    "Found exact match: Account {TargetAccountId} contains all {Count} creatives from account {SourceAccountId}",
                    accountsWithAllCreatives, externalIds.Count, currentAccountId);
                return accountsWithAllCreatives;
            }

            _logger.LogDebug(
                "Account {AccountId} has {ActualCount} creatives, expected {ExpectedCount}. Not an exact match",
                accountsWithAllCreatives, foundAccountCreativeCount, externalIds.Count);
        }

        return null;
    }

    public async Task MergeAccounts(
        long targetAccountId,
        long sourceAccountId,
        CancellationToken cancellationToken = default)
    {
        if (targetAccountId == sourceAccountId)
        {
            _logger.LogWarning("Cannot merge account {AccountId} with itself", targetAccountId);
            return;
        }

        _logger.LogInformation(
            "Starting merge: Source account {SourceAccountId} -> Target account {TargetAccountId}",
            sourceAccountId, targetAccountId);

        await using var context = _contextFactory();

        // Start transaction for atomic operation
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var now = DateTimeOffset.UtcNow;

            // Strategy: First delete duplicates, then update remaining records
            // This ensures no constraint violations

            // First, remove all duplicate records from source that exist in target
            await RemoveDuplicateEntities(context, targetAccountId, sourceAccountId, cancellationToken);

            // Now safely update all remaining records without conflicts

            // Update VkOrdCreatives
            var creativesUpdated = await context.VkOrdCreatives
                .Where(e => e.LogicalAccountId == sourceAccountId && !e.IsDeleted)
                .ExecuteUpdateAsync(
                    setters => setters
                        .SetProperty(e => e.LogicalAccountId, targetAccountId)
                        .SetProperty(e => e.UpdatedAt, now),
                    cancellationToken);

            _logger.LogDebug("Updated {Count} creatives", creativesUpdated);

            // Update VkOrdContracts
            var contractsUpdated = await context.VkOrdContracts
                .Where(e => e.LogicalAccountId == sourceAccountId && !e.IsDeleted)
                .ExecuteUpdateAsync(
                    setters => setters
                        .SetProperty(e => e.LogicalAccountId, targetAccountId)
                        .SetProperty(e => e.UpdatedAt, now),
                    cancellationToken);

            _logger.LogDebug("Updated {Count} contracts", contractsUpdated);

            // Update VkOrdCounterparties
            var counterpartiesUpdated = await context.VkOrdCounterparties
                .Where(e => e.LogicalAccountId == sourceAccountId && !e.IsDeleted)
                .ExecuteUpdateAsync(
                    setters => setters
                        .SetProperty(e => e.LogicalAccountId, targetAccountId)
                        .SetProperty(e => e.UpdatedAt, now),
                    cancellationToken);

            _logger.LogDebug("Updated {Count} counterparties", counterpartiesUpdated);

            // Update VkOrdMedia
            var mediaUpdated = await context.VkOrdMedias
                .Where(e => e.LogicalAccountId == sourceAccountId && !e.IsDeleted)
                .ExecuteUpdateAsync(
                    setters => setters
                        .SetProperty(e => e.LogicalAccountId, targetAccountId)
                        .SetProperty(e => e.UpdatedAt, now),
                    cancellationToken);

            _logger.LogDebug("Updated {Count} media", mediaUpdated);

            // Update VkOrdInvoices
            var invoicesUpdated = await context.VkOrdInvoices
                .Where(e => e.LogicalAccountId == sourceAccountId && !e.IsDeleted)
                .ExecuteUpdateAsync(
                    setters => setters
                        .SetProperty(e => e.LogicalAccountId, targetAccountId)
                        .SetProperty(e => e.UpdatedAt, now),
                    cancellationToken);

            _logger.LogDebug("Updated {Count} invoices", invoicesUpdated);

            // Update VkOrdStatistics
            var statisticsUpdated = await context.VkOrdStatistics
                .Where(e => e.LogicalAccountId == sourceAccountId && !e.IsDeleted)
                .ExecuteUpdateAsync(
                    setters => setters
                        .SetProperty(e => e.LogicalAccountId, targetAccountId)
                        .SetProperty(e => e.UpdatedAt, now),
                    cancellationToken);

            _logger.LogDebug("Updated {Count} statistics", statisticsUpdated);

            // Update VkOrdErirStatuses
            var erirStatusesUpdated = await context.VkOrdErirStatuses
                .Where(e => e.LogicalAccountId == sourceAccountId)
                .ExecuteUpdateAsync(
                    setters => setters
                        .SetProperty(e => e.LogicalAccountId, targetAccountId)
                        .SetProperty(e => e.UpdatedAt, now),
                    cancellationToken);

            _logger.LogDebug("Updated {Count} ERIR statuses", erirStatusesUpdated);

            // Update ApiCredentials to point to target account
            var credentialsUpdated = await context.ApiCredentials
                .Where(a => a.LogicalAccountId == sourceAccountId)
                .ExecuteUpdateAsync(
                    setters => setters
                        .SetProperty(a => a.LogicalAccountId, targetAccountId)
                        .SetProperty(a => a.UpdatedAt, now),
                    cancellationToken);

            _logger.LogDebug("Updated {Count} API credentials", credentialsUpdated);

            // Delete the source logical account as it's no longer needed
            var logicalAccountDeleted = await context.VkLogicalAccounts
                .Where(la => la.Id == sourceAccountId)
                .ExecuteDeleteAsync(cancellationToken);

            if (logicalAccountDeleted > 0)
            {
                _logger.LogDebug("Deleted source logical account {SourceAccountId}", sourceAccountId);
            }

            await transaction.CommitAsync(cancellationToken);

            _logger.LogInformation(
                "Successfully merged account {SourceAccountId} into {TargetAccountId}. " +
                "Updated: {Creatives} creatives, {Contracts} contracts, {Counterparties} counterparties, " +
                "{Media} media, {Invoices} invoices, {Statistics} statistics, {ErirStatuses} ERIR statuses, " +
                "{Credentials} API credentials. Deleted source logical account: {LogicalAccountDeleted}",
                sourceAccountId, targetAccountId,
                creativesUpdated, contractsUpdated, counterpartiesUpdated,
                mediaUpdated, invoicesUpdated, statisticsUpdated, erirStatusesUpdated,
                credentialsUpdated, logicalAccountDeleted > 0);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error merging account {SourceAccountId} into {TargetAccountId}",
                sourceAccountId, targetAccountId);

            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private async Task RemoveDuplicateEntities(
        AppDbContext context,
        long targetAccountId,
        long sourceAccountId,
        CancellationToken cancellationToken)
    {
        // Remove duplicate Creatives
        var duplicateCreativeIds = await context.VkOrdCreatives
            .Where(c => c.LogicalAccountId == sourceAccountId && !c.IsDeleted)
            .Select(c => c.ExternalId)
            .Intersect(
                context.VkOrdCreatives
                    .Where(c => c.LogicalAccountId == targetAccountId && !c.IsDeleted)
                    .Select(c => c.ExternalId)
            )
            .ToListAsync(cancellationToken);

        if (duplicateCreativeIds.Count > 0)
        {
            var deleted = await context.VkOrdCreatives
                .Where(c => c.LogicalAccountId == sourceAccountId &&
                           duplicateCreativeIds.Contains(c.ExternalId))
                .ExecuteDeleteAsync(cancellationToken);

            _logger.LogDebug("Removed {Count} duplicate creatives", deleted);
        }

        // Remove duplicate Contracts
        var duplicateContractIds = await context.VkOrdContracts
            .Where(c => c.LogicalAccountId == sourceAccountId && !c.IsDeleted)
            .Select(c => c.ExternalId)
            .Intersect(
                context.VkOrdContracts
                    .Where(c => c.LogicalAccountId == targetAccountId && !c.IsDeleted)
                    .Select(c => c.ExternalId)
            )
            .ToListAsync(cancellationToken);

        if (duplicateContractIds.Count > 0)
        {
            var deleted = await context.VkOrdContracts
                .Where(c => c.LogicalAccountId == sourceAccountId &&
                           duplicateContractIds.Contains(c.ExternalId))
                .ExecuteDeleteAsync(cancellationToken);

            _logger.LogDebug("Removed {Count} duplicate contracts", deleted);
        }

        // Remove duplicate Counterparties
        var duplicateCounterpartyIds = await context.VkOrdCounterparties
            .Where(c => c.LogicalAccountId == sourceAccountId && !c.IsDeleted)
            .Select(c => c.ExternalId)
            .Intersect(
                context.VkOrdCounterparties
                    .Where(c => c.LogicalAccountId == targetAccountId && !c.IsDeleted)
                    .Select(c => c.ExternalId)
            )
            .ToListAsync(cancellationToken);

        if (duplicateCounterpartyIds.Count > 0)
        {
            var deleted = await context.VkOrdCounterparties
                .Where(c => c.LogicalAccountId == sourceAccountId &&
                           duplicateCounterpartyIds.Contains(c.ExternalId))
                .ExecuteDeleteAsync(cancellationToken);

            _logger.LogDebug("Removed {Count} duplicate counterparties", deleted);
        }

        // Remove duplicate Media
        var duplicateMediaIds = await context.VkOrdMedias
            .Where(m => m.LogicalAccountId == sourceAccountId && !m.IsDeleted)
            .Select(m => m.ExternalId)
            .Intersect(
                context.VkOrdMedias
                    .Where(m => m.LogicalAccountId == targetAccountId && !m.IsDeleted)
                    .Select(m => m.ExternalId)
            )
            .ToListAsync(cancellationToken);

        if (duplicateMediaIds.Count > 0)
        {
            var deleted = await context.VkOrdMedias
                .Where(m => m.LogicalAccountId == sourceAccountId &&
                           duplicateMediaIds.Contains(m.ExternalId))
                .ExecuteDeleteAsync(cancellationToken);

            _logger.LogDebug("Removed {Count} duplicate media", deleted);
        }

        // Remove duplicate Invoices
        var duplicateInvoiceIds = await context.VkOrdInvoices
            .Where(i => i.LogicalAccountId == sourceAccountId && !i.IsDeleted)
            .Select(i => i.ExternalId)
            .Intersect(
                context.VkOrdInvoices
                    .Where(i => i.LogicalAccountId == targetAccountId && !i.IsDeleted)
                    .Select(i => i.ExternalId)
            )
            .ToListAsync(cancellationToken);

        if (duplicateInvoiceIds.Count > 0)
        {
            var deleted = await context.VkOrdInvoices
                .Where(i => i.LogicalAccountId == sourceAccountId &&
                           duplicateInvoiceIds.Contains(i.ExternalId))
                .ExecuteDeleteAsync(cancellationToken);

            _logger.LogDebug("Removed {Count} duplicate invoices", deleted);
        }

        // Remove duplicate Statistics (check by CreativeExternalId and PadExternalId)
        var duplicateStatistics = await context.VkOrdStatistics
            .Where(s => s.LogicalAccountId == sourceAccountId && !s.IsDeleted)
            .Join(
                context.VkOrdStatistics.Where(s => s.LogicalAccountId == targetAccountId && !s.IsDeleted),
                s => new { s.CreativeExternalId, s.PadExternalId },
                s => new { s.CreativeExternalId, s.PadExternalId },
                (source, target) => source.Id
            )
            .ToListAsync(cancellationToken);

        if (duplicateStatistics.Any())
        {
            var deleted = await context.VkOrdStatistics
                .Where(s => duplicateStatistics.Contains(s.Id))
                .ExecuteDeleteAsync(cancellationToken);

            _logger.LogDebug("Removed {Count} duplicate statistics", deleted);
        }

        // Remove duplicate ERIR Statuses
        var duplicateErirStatuses = await context.VkOrdErirStatuses
            .Where(e => e.LogicalAccountId == sourceAccountId)
            .Join(
                context.VkOrdErirStatuses.Where(e => e.LogicalAccountId == targetAccountId),
                e => new { e.EntityType, e.ExternalId },
                e => new { e.EntityType, e.ExternalId },
                (source, target) => source.Id
            )
            .ToListAsync(cancellationToken);

        if (duplicateErirStatuses.Count > 0)
        {
            var deleted = await context.VkOrdErirStatuses
                .Where(e => duplicateErirStatuses.Contains(e.Id))
                .ExecuteDeleteAsync(cancellationToken);

            _logger.LogDebug("Removed {Count} duplicate ERIR statuses", deleted);
        }
    }
}