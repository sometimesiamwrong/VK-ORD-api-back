using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

/// <summary>
/// Refresh токены с ротацией
/// </summary>
public class RefreshToken : EntityBase
{
    /// <summary>
    /// Хэш токена
    /// </summary>
    [Required]
    [MaxLength(256)]
    public string TokenHash { get; set; } = string.Empty;

    /// <summary>
    /// Срок действия токена
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// IP адрес создания токена
    /// </summary>
    [MaxLength(64)]
    public string? CreatedByIp { get; set; }

    /// <summary>
    /// ID устройства
    /// </summary>
    [MaxLength(128)]
    public string? DeviceId { get; set; }

    /// <summary>
    /// Дата revocation токена
    /// </summary>
    public DateTime? RevokedAt { get; set; }

    /// <summary>
    /// Хэш замененного токена
    /// </summary>
    [MaxLength(256)]
    public string? ReplacedByTokenHash { get; set; }

    /// <summary>
    /// Флаг revocation токена
    /// </summary>
    public bool IsRevoked => RevokedAt.HasValue;

    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    [Required]
    public long UserId { get; set; }

    /// <summary>
    /// Пользователь
    /// </summary>
    public virtual User? User { get; set; }
}


