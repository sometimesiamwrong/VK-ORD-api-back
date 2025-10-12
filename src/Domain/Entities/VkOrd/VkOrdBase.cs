using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using Microsoft.Net.Http.Headers;

namespace Domain.Entities.VkOrd;

/// <summary>
/// Базовая сущность для хранения данных VK ORD API
/// Использует составной ключ (LogicalAccountId, ExternalId) для изоляции данных пользователей
/// </summary>
public abstract class VkOrdBase
{
    /// <summary>
    /// Автоинкрементный идентификатор для связей
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Идентификатор суммаризирующей сущности
    /// </summary>
    [Required]
    public required long LogicalAccountId { get; set; }

    /// <summary>
    /// Внешний идентификатор из VK ORD API
    /// </summary>
    [Required]
    [MaxLength(255)]
    public required string ExternalId { get; set; }

    /// <summary>
    /// Дата обновления данных
    /// </summary>
    [Required]
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    
    /// <summary>
    /// Дата создания данных
    /// </summary>
    [Required]
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Дата истечения актуальности данных
    /// </summary>
    [Required]
    public virtual DateTimeOffset ExpiresAt { get; set; } = DateTimeOffset.UtcNow.AddMinutes(60);

    /// <summary>
    /// Версия данных
    /// </summary>
    [Required]
    public int Version { get; set; } = 1;

    /// <summary>
    /// JSON данные из VK ORD API
    /// </summary>
    [Required]
    [Column(TypeName = "jsonb")]
    public string JsonData { get; set; } = null!;

    /// <summary>
    /// Хеш данных для проверки актуальности
    /// </summary>
    [MaxLength(64)]
    public string DataHash { get; set; } = null!;

    /// <summary>
    /// Удалена ли сущность
    /// </summary>
    public bool IsDeleted { get; set; } = false;

    /// <summary>
    /// Суммаризирующая сущность
    /// </summary>
    [ForeignKey(nameof(LogicalAccountId))]
    public virtual VkOrdLogicalAccount OrdLogicalAccount { get; set; } = null!;

    /// <summary>
    /// Проверка актуальности данных
    /// </summary>
    /// <returns>True если кэш актуален</returns>
    public bool IsExpired()
    {
        return DateTimeOffset.UtcNow > ExpiresAt;
    }
    
    public bool IsNew() => Id == 0;

    /// <summary>
    /// Проверка необходимости обновления данных
    /// </summary>
    /// <param name="threshold">Порог обновления (0.0-1.0)</param>
    /// <returns>True если нужно обновить данные</returns>
    public bool ShouldRefresh(double threshold = 0.8)
    {
        var totalTtl = ExpiresAt - UpdatedAt;
        var elapsed = DateTimeOffset.UtcNow - UpdatedAt;
        var ratio = elapsed.TotalMilliseconds / totalTtl.TotalMilliseconds;
        return ratio >= threshold;
    }

    /// <summary>
    /// Обновление данных
    /// </summary>
    /// <param name="data">Новые данные</param>
    /// <param name="ttlMinutes">Время жизни в минутах</param>
    public void UpdateData<T>(T data, int ttlMinutes = 60)
    {
        SetData(data);
        UpdatedAt = DateTimeOffset.UtcNow;
        ExpiresAt = UpdatedAt.AddMinutes(ttlMinutes);
        Version++;
        DataHash = ComputeHash(JsonData);
    }

    /// <summary>
    /// Получение данных из JSON
    /// </summary>
    /// <typeparam name="T">Тип данных</typeparam>
    /// <returns>Данные</returns>
    public virtual T GetData<T>()
    {
        return JsonSerializer.Deserialize<T>(JsonData)!;
    }

    /// <summary>
    /// Установка данных в JSON
    /// </summary>
    /// <param name="data">Данные</param>
    public virtual void SetData<T>(T data)
    {
        JsonData = JsonSerializer.Serialize(data);
    }

    /// <summary>
    /// Вычисление хеша данных
    /// </summary>
    /// <param name="data">Данные для хеширования</param>
    /// <returns>SHA256 хеш</returns>
    private static string ComputeHash(string data)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var hashBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(data));
        return Convert.ToHexString(hashBytes);
    }
}
