using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.VkOrdCache;

/// <summary>
/// Базовая сущность для кэширования данных VK ORD API
/// Использует составной ключ (ApiCredentialId, ExternalId) для изоляции данных пользователей
/// </summary>
public abstract class VkOrdCacheBase
{
    /// <summary>
    /// Автоинкрементный идентификатор для связей
    /// </summary>
    [Key]
    public long Id { get; set; }

    /// <summary>
    /// Идентификатор API учетных данных
    /// </summary>
    [Required]
    public long ApiCredentialId { get; set; }

    /// <summary>
    /// Внешний идентификатор из VK ORD API
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string ExternalId { get; set; } = string.Empty;

    /// <summary>
    /// Дата кэширования данных
    /// </summary>
    [Required]
    public DateTimeOffset CachedAt { get; set; }

    /// <summary>
    /// Дата истечения кэша (TTL)
    /// </summary>
    [Required]
    public DateTimeOffset ExpiresAt { get; set; }

    /// <summary>
    /// Версия данных в кэше
    /// </summary>
    [Required]
    public int Version { get; set; } = 1;

    /// <summary>
    /// JSON данные из VK ORD API
    /// </summary>
    [Required]
    [Column(TypeName = "jsonb")]
    public string JsonData { get; set; } = string.Empty;

    /// <summary>
    /// Хеш данных для проверки изменений
    /// </summary>
    [MaxLength(64)]
    public string? DataHash { get; set; }

    /// <summary>
    /// API учетные данные
    /// </summary>
    [ForeignKey(nameof(ApiCredentialId))]
    public virtual ApiCredential ApiCredential { get; set; } = null!;

    /// <summary>
    /// Проверка актуальности кэша
    /// </summary>
    /// <returns>True если кэш актуален</returns>
    public bool IsExpired()
    {
        return DateTimeOffset.UtcNow > ExpiresAt;
    }

    /// <summary>
    /// Проверка необходимости обновления кэша
    /// </summary>
    /// <param name="threshold">Порог обновления (0.0-1.0)</param>
    /// <returns>True если нужно обновить кэш</returns>
    public bool ShouldRefresh(double threshold = 0.8)
    {
        var totalTtl = ExpiresAt - CachedAt;
        var elapsed = DateTimeOffset.UtcNow - CachedAt;
        var ratio = elapsed.TotalMilliseconds / totalTtl.TotalMilliseconds;
        return ratio >= threshold;
    }

    /// <summary>
    /// Обновление данных кэша
    /// </summary>
    /// <param name="jsonData">Новые JSON данные</param>
    /// <param name="ttlMinutes">Время жизни в минутах</param>
    public void UpdateCache(string jsonData, int ttlMinutes = 60)
    {
        JsonData = jsonData;
        CachedAt = DateTimeOffset.UtcNow;
        ExpiresAt = CachedAt.AddMinutes(ttlMinutes);
        Version++;
        DataHash = ComputeHash(jsonData);
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
