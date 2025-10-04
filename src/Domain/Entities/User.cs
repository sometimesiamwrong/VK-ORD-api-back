using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

/// <summary>
/// Приложенческий пользователь
/// </summary>
public class User : EntityBase
{
    /// <summary>
    /// Имя пользователя
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Имя пользователя
    /// </summary>
    [MaxLength(200)]
    public string? Name { get; set; }

    /// <summary>
    /// Хэш пароля
    /// </summary>
    [Required]
    [MaxLength(500)]
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// Флаг активности пользователя
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Список учетных данных пользователя
    /// </summary>
    public ICollection<ApiCredential> ApiCredentials { get; set; } = new List<ApiCredential>();

    /// <summary>
    /// Список refresh токенов пользователя
    /// </summary>
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>(); 
}