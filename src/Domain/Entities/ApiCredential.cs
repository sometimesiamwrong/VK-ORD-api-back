using System.ComponentModel.DataAnnotations;
using Domain.Entities;
using Domain.Entities.Enums;

namespace Domain.Entities; 

/// <summary>
/// VK ORD учетные данные пользователя
/// </summary>
public class ApiCredential : EntityBase
{
    /// <summary>
    /// Среда
    /// </summary>
    [Required]
    public VkOrdEnvironmentCode Environment { get; set; } = VkOrdEnvironmentCode.Sandbox;

    /// <summary>
    /// Зашифрованный токен
    /// </summary>
    [Required]
    public required string TokenEncrypted { get; set; }

    /// <summary>
    /// Имя учетных данных
    /// </summary>
    [MaxLength(150)]
    public string? DisplayName { get; set; }

    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// Пользователь
    /// </summary>
    [Required]
    public virtual User? User { get; set; }
}


