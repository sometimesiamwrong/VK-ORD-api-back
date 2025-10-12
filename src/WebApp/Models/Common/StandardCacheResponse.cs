using WebApp.Models.Counterparties;

namespace WebApp.Models.Common;

/// <summary>
/// Стандартизированный ответ с кэшированием и данными
/// </summary>
/// <typeparam name="TData">Тип данных</typeparam>
public class StandardCacheResponse<TData> : CacheResponse
{
    /// <summary>
    /// Данные ответа
    /// </summary>
    public List<TData> Data { get; set; } = new();

    /// <summary>
    /// Общее количество найденных элементов
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Количество возвращенных элементов
    /// </summary>
    public int ReturnedCount { get; set; }
}

/// <summary>
/// Стандартизированный ответ с одним элементом
/// </summary>
/// <typeparam name="TData">Тип данных</typeparam>й
public class StandardSingleCacheResponse<TData> : CacheResponse
{
    /// <summary>
    /// Элемент данных
    /// </summary>
    public TData? Item { get; set; }
}

