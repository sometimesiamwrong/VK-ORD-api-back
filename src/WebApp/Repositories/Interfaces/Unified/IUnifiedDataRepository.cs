using WebApp.Models.Common;
using WebApp.Models.Counterparties;

namespace WebApp.Repositories.Interfaces.Unified;

/// <summary>
/// Унифицированный репозиторий для получения данных с тремя уровнями доступа
/// </summary>
/// <typeparam name="TEntity">Тип сущности</typeparam>
/// <typeparam name="TDto">Тип DTO</typeparam>
public interface IUnifiedDataRepository<TEntity, TDto> where TEntity : class
{
    /// <summary>
    /// Получить данные с учетом кэширования
    /// </summary>
    /// <param name="apiCredentialId">ID учетных данных API</param>
    /// <param name="key">Ключ для поиска данных</param>
    /// <param name="request">Параметры запроса с кэшированием</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Результат с данными и метаинформацией</returns>
    Task<UnifiedDataResult<TDto>> GetDataAsync(
        long apiCredentialId,
        string key,
        CacheRequest request,
        CancellationToken cancellationToken);

    /// <summary>
    /// Сохранить данные в кэш и БД
    /// </summary>
    /// <param name="apiCredentialId">ID учетных данных API</param>
    /// <param name="key">Ключ для сохранения</param>
    /// <param name="data">Данные для сохранения</param>
    /// <param name="ttlMinutes">Время жизни кэша в минутах</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task SaveDataAsync(
        long apiCredentialId,
        string key,
        IEnumerable<TDto> data,
        int ttlMinutes,
        CancellationToken cancellationToken);
}

/// <summary>
/// Результат получения данных
/// </summary>
/// <typeparam name="TDto">Тип DTO</typeparam>
public class UnifiedDataResult<TDto>
{
    /// <summary>
    /// Полученные данные
    /// </summary>
    public List<TDto> Data { get; set; } = new();

    /// <summary>
    /// Источник данных
    /// </summary>
    public DataSource Source { get; set; }

    /// <summary>
    /// Время получения данных
    /// </summary>
    public DateTimeOffset RetrievedAt { get; set; }

    /// <summary>
    /// Время истечения кэша
    /// </summary>
    public DateTimeOffset? CacheExpiresAt { get; set; }

    /// <summary>
    /// Время создания кэша
    /// </summary>
    public DateTimeOffset? CacheCreatedAt { get; set; }

    /// <summary>
    /// Версия кэша
    /// </summary>
    public int CacheVersion { get; set; }

    /// <summary>
    /// Хеш данных
    /// </summary>
    public string? DataHash { get; set; }

    /// <summary>
    /// Статистика кэширования
    /// </summary>
    public CacheStatistics CacheStatistics { get; set; } = new();

    /// <summary>
    /// Была ли операция успешной
    /// </summary>
    public bool Success { get; set; } = true;

    /// <summary>
    /// Сообщение об ошибке
    /// </summary>
    public string? ErrorMessage { get; set; }
}

