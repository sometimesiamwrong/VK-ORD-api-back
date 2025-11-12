namespace Domain.Services.Interfaces;

public interface ICacheService
{
    Task<TEntity?> Get<TEntity>(string key, CancellationToken cancellationToken) where TEntity : class;
    Task<IEnumerable<TEntity?>> GetList<TEntity>(IEnumerable<string> keys, CancellationToken cancellationToken) where TEntity : class;
    Task Save<TEntity>(string key, TEntity entity, CancellationToken cancellationToken) where TEntity : class;
    Task SaveList<TEntity>(IEnumerable<string> keys, IEnumerable<TEntity> entities, CancellationToken cancellationToken) where TEntity : class;
    Task Remove<TEntity>(string key, CancellationToken cancellationToken) where TEntity : class;
    Task RemoveList<TEntity>(IEnumerable<string> keys, CancellationToken cancellationToken) where TEntity : class;
    Task Clear<TEntity>(CancellationToken cancellationToken) where TEntity : class;
    Task ClearList<TEntity>(CancellationToken cancellationToken) where TEntity : class;
    Task ClearAll<TEntity>(CancellationToken cancellationToken) where TEntity : class;
}