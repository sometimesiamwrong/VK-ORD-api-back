using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.Services.Interfaces;

/// <summary>
/// Service for merging logical accounts when duplicate creatives are detected
/// </summary>
public interface ILogicalAccountMergeService
{
    /// <summary>
    /// Finds a logical account that contains all specified creative external IDs
    /// </summary>
    /// <param name="externalIds">List of creative external IDs to search for</param>
    /// <param name="currentAccountId">Current logical account ID to exclude from search</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Logical account ID if found, null otherwise</returns>
    Task<long?> FindAccountWithAllCreatives(
        List<string> externalIds,
        long currentAccountId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Merges all entities from source account to target account
    /// </summary>
    /// <param name="targetAccountId">Target logical account ID to merge into</param>
    /// <param name="sourceAccountId">Source logical account ID to merge from</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task MergeAccounts(
        long targetAccountId,
        long sourceAccountId,
        CancellationToken cancellationToken = default);
}