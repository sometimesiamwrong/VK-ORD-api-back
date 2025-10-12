using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using WebApp.Services.Interfaces;

namespace WebApp.Services.Implementations.Cache
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly ILogger<CacheService> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public CacheService(IDistributedCache distributedCache, ILogger<CacheService> logger)
        {
            _distributedCache = distributedCache;
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                WriteIndented = false
            };
        }

        public async Task<TEntity?> Get<TEntity>(string key, CancellationToken cancellationToken) where TEntity : class
        {
            try
            {
                var value = await _distributedCache.GetStringAsync(key, cancellationToken);
                if (string.IsNullOrEmpty(value))
                    return null;

                return JsonSerializer.Deserialize<TEntity>(value, _jsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cache value for key {Key}", key);
                return null;
            }
        }

        public async Task<IEnumerable<TEntity?>> GetList<TEntity>(IEnumerable<string> keys, CancellationToken cancellationToken) where TEntity : class
        {
            var results = new List<TEntity?>();
            foreach (var key in keys)
            {
                var value = await Get<TEntity>(key, cancellationToken);
                results.Add(value);
            }
            return results;
        }

        public async Task Remove<TEntity>(string key, CancellationToken cancellationToken) where TEntity : class
        {
            try
            {
                await _distributedCache.RemoveAsync(key, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cache value for key {Key}", key);
            }
        }

        public async Task RemoveList<TEntity>(IEnumerable<string> keys, CancellationToken cancellationToken) where TEntity : class
        {
            foreach (var key in keys)
            {
                await Remove<TEntity>(key, cancellationToken);
            }
        }

        public async Task Save<TEntity>(string key, TEntity entity, CancellationToken cancellationToken) where TEntity : class
        {
            try
            {
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
                };

                var value = JsonSerializer.Serialize(entity, _jsonOptions);
                await _distributedCache.SetStringAsync(key, value, options, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving cache value for key {Key}", key);
            }
        }

        public async Task SaveList<TEntity>(IEnumerable<string> keys, IEnumerable<TEntity> entities, CancellationToken cancellationToken) where TEntity : class
        {
            var keyList = keys.ToList();
            var entityList = entities.ToList();
            
            for (int i = 0; i < Math.Min(keyList.Count, entityList.Count); i++)
            {
                await Save(keyList[i], entityList[i], cancellationToken);
            }
        }

        public async Task Clear<TEntity>(CancellationToken cancellationToken) where TEntity : class
        {
            // Redis не поддерживает очистку по типу, поэтому просто логируем
            _logger.LogWarning("Clear<TEntity> called but Redis doesn't support type-based clearing");
            await Task.CompletedTask;
        }

        public async Task ClearAll<TEntity>(CancellationToken cancellationToken) where TEntity : class
        {
            // Redis не поддерживает очистку по типу, поэтому просто логируем
            _logger.LogWarning("ClearAll<TEntity> called but Redis doesn't support type-based clearing");
            await Task.CompletedTask;
        }

        public async Task ClearList<TEntity>(CancellationToken cancellationToken) where TEntity : class
        {
            // Redis не поддерживает очистку по типу, поэтому просто логируем
            _logger.LogWarning("ClearList<TEntity> called but Redis doesn't support type-based clearing");
            await Task.CompletedTask;
        }

    }
}