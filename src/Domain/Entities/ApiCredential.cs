using System.ComponentModel.DataAnnotations;
using Domain.Entities.Enums;
using Domain.Entities.VkOrd;

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
    public VkOrdApiEnvironmentCode ApiEnvironment { get; set; } = VkOrdApiEnvironmentCode.Sandbox;

    /// <summary>
    /// Идентификатор логического аккаунта
    /// </summary>
    [Required]
    public long LogicalAccountId { get; set; }

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

    /// <summary>
    /// Логический аккаунт
    /// </summary>
    [Required]
    public virtual VkOrdLogicalAccount? LogicalAccount { get; set; }

    /// <summary>
    /// Список шаблонов потоков для этих учетных данных
    /// </summary>
    public ICollection<FlowTemplate> FlowTemplates { get; set; } = new List<FlowTemplate>();
}




