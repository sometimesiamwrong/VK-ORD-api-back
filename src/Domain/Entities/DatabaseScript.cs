using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

/// <summary>
/// Сущность для отслеживания выполненных SQL-скриптов
/// </summary>
public class DatabaseScript
{
    /// <summary>
    /// Идентификатор скрипта
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Имя скрипта
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string ScriptName { get; set; } = string.Empty;
   
    /// <summary>
    /// Хэш скрипта
    /// </summary>
    [Required]
    [MaxLength(64)]
    public string ScriptHash { get; set; } = string.Empty;

    /// <summary>
    /// Дата выполнения скрипта
    /// </summary>
    public DateTime ExecutedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Описание скрипта
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Флаг успешности выполнения скрипта
    /// </summary>
    public bool IsSuccessful { get; set; } = true;

    /// <summary>
    /// Сообщение об ошибке
    /// </summary>
    public string? ErrorMessage { get; set; }

    public bool IsNewOrUpdate()
    {
        return Id == Guid.Empty;
    }
}
