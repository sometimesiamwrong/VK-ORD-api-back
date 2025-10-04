using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using WebApp.Configuration;

namespace WebApp.Services.Implementations
{
    /// <summary>
    /// Декоратор для IDistributedCache, ограничивающий время операций.
    /// Чтение: по умолчанию 200 мс. Запись/удаление/refresh: 400 мс.
    /// При таймауте чтения возвращается cache-miss (null), при таймауте записи/удаления — логируем и продолжаем.
    /// </summary>
    public class TimeoutDistributedCache : IDistributedCache
    {
        private readonly IDistributedCache _inner;
        private readonly ILogger<TimeoutDistributedCache> _logger;
        private readonly TimeSpan _readTimeout;
        private readonly TimeSpan _writeTimeout;

        public TimeoutDistributedCache(
            IDistributedCache inner,
            IOptions<RedisConfiguration> options,
            ILogger<TimeoutDistributedCache> logger)
        {
            _inner = inner;
            _logger = logger;
            var config = options.Value;
            _readTimeout = TimeSpan.FromMilliseconds(config.ReadTimeoutMs <= 0 ? 200 : config.ReadTimeoutMs);
            _writeTimeout = TimeSpan.FromMilliseconds(config.WriteTimeoutMs <= 0 ? 400 : config.WriteTimeoutMs);
        }

        // ---------- Async API (используется в коде) ----------
        public Task<byte[]?> GetAsync(string key, CancellationToken token = default)
        {
            return ExecuteWithTimeout(
                () => _inner.GetAsync(key, token),
                _readTimeout,
                onTimeout: () => _logger.LogWarning("Cache read timeout for key {Key} after {TimeoutMs} ms", key, (int)_readTimeout.TotalMilliseconds),
                onError: ex => _logger.LogWarning(ex, "Cache read failed for key {Key}", key),
                defaultValue: (byte[]?)null);
        }

        public Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default)
        {
            return ExecuteWithTimeout(
                () => _inner.SetAsync(key, value, options, token),
                _writeTimeout,
                onTimeout: () => _logger.LogWarning("Cache write timeout for key {Key} after {TimeoutMs} ms", key, (int)_writeTimeout.TotalMilliseconds),
                onError: ex => _logger.LogWarning(ex, "Cache write failed for key {Key}", key));
        }

        public Task RefreshAsync(string key, CancellationToken token = default)
        {
            return ExecuteWithTimeout(
                () => _inner.RefreshAsync(key, token),
                _writeTimeout,
                onTimeout: () => _logger.LogWarning("Cache refresh timeout for key {Key} after {TimeoutMs} ms", key, (int)_writeTimeout.TotalMilliseconds),
                onError: ex => _logger.LogWarning(ex, "Cache refresh failed for key {Key}", key));
        }

        public Task RemoveAsync(string key, CancellationToken token = default)
        {
            return ExecuteWithTimeout(
                () => _inner.RemoveAsync(key, token),
                _writeTimeout,
                onTimeout: () => _logger.LogWarning("Cache remove timeout for key {Key} after {TimeoutMs} ms", key, (int)_writeTimeout.TotalMilliseconds),
                onError: ex => _logger.LogWarning(ex, "Cache remove failed for key {Key}", key));
        }

        private async Task<TResult?> ExecuteWithTimeout<TResult>(
            Func<Task<TResult>> operation,
            TimeSpan timeout,
            Action? onTimeout = null,
            Action<Exception>? onError = null,
            TResult? defaultValue = default)
        {
            var opTask = operation();
            var completed = await Task.WhenAny(opTask, Task.Delay(timeout)).ConfigureAwait(false);
            if (completed == opTask)
            {
                try
                {
                    return await opTask.ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    onTimeout?.Invoke();
                    return defaultValue;
                }
                catch (Exception ex)
                {
                    onError?.Invoke(ex);
                    return defaultValue;
                }
            }
            else
            {
                // таймаут — наблюдаем исключение, если оно будет, чтобы не было UnobservedTaskException
                _ = opTask.ContinueWith(t => { var _ = t.Exception; }, TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnFaulted);
                onTimeout?.Invoke();
                return defaultValue;
            }
        }

        private async Task ExecuteWithTimeout(
            Func<Task> operation,
            TimeSpan timeout,
            Action? onTimeout = null,
            Action<Exception>? onError = null)
        {
            var opTask = operation();
            var completed = await Task.WhenAny(opTask, Task.Delay(timeout)).ConfigureAwait(false);
            if (completed == opTask)
            {
                try
                {
                    await opTask.ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    onTimeout?.Invoke();
                }
                catch (Exception ex)
                {
                    onError?.Invoke(ex);
                }
            }
            else
            {
                // таймаут — наблюдаем исключение, если оно будет, чтобы не было UnobservedTaskException
                _ = opTask.ContinueWith(t => { var _ = t.Exception; }, TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnFaulted);
                onTimeout?.Invoke();
            }
        }

        // ---------- Sync API (редко используется; ограничим через блокировку) ----------
        public byte[]? Get(string key)
        {
            try
            {
                return GetAsync(key).GetAwaiter().GetResult();
            }
            catch (OperationCanceledException)
            {
                return null;
            }
        }

        public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
        {
            try
            {
                SetAsync(key, value, options).GetAwaiter().GetResult();
            }
            catch (OperationCanceledException)
            {
                // swallow
            }
        }

        public void Refresh(string key)
        {
            try
            {
                RefreshAsync(key).GetAwaiter().GetResult();
            }
            catch (OperationCanceledException)
            {
                // swallow
            }
        }

        public void Remove(string key)
        {
            try
            {
                RemoveAsync(key).GetAwaiter().GetResult();
            }
            catch (OperationCanceledException)
            {
                // swallow
            }
        }
    }
}


