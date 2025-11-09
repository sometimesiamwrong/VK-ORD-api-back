using Domain.Entities;
using Domain.Entities.VkOrd;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Services.Interfaces;

/// <summary>
/// Сервис для работы с данными VK ORD API с кэшированием
/// </summary>
/// <typeparam name="TEntity">Тип сущности VkOrd</typeparam>
/// <typeparam name="TApiResponse">Тип ответа API</typeparam>
public interface IVkOrdDataService<TEntity, TApiResponse>
    where TEntity : VkOrdBase
    where TApiResponse : class
{
    /// <summary>
    /// Получить данные с учетом кэширования и работы с БД
    /// </summary>
    /// <param name="identifier">Идентификатор (externalId, erid и т.д.)</param>
    /// <param name="getApiOperation">Функция для получения данных из API</param>
    /// <param name="mapOperation">Функция для маппинга данных</param>
    /// <param name="getFromDatabaseQuery">Функция для создания запроса к БД</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Сущность с данными</returns>
    Task<TEntity> GetAsync(
        string identifier,
        Func<string, CancellationToken, Task<TApiResponse?>> getApiOperation,
        Func<TApiResponse, TEntity?, ApiCredential, TEntity> mapOperation,
        Func<DbSet<TEntity>, IQueryable<TEntity>> getFromDatabaseQuery,
        CancellationToken cancellationToken);

    /// <summary>
    /// Выполнить запрос и получить первую запись
    /// </summary>
    /// <param name="query">Запрос к БД</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Первая найденная сущность</returns>
    Task<TEntity?> FirstOrDefaultAsync(IQueryable<TEntity> query, CancellationToken cancellationToken);

    /// <summary>
    /// Выполнить запрос и получить список записей
    /// </summary>
    /// <param name="query">Запрос к БД</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Список найденных сущностей</returns>
    Task<List<TEntity>> ToListAsync(IQueryable<TEntity> query, CancellationToken cancellationToken);
}
