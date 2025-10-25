using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Entities.Enums;

namespace Domain.Entities;

/// <summary>
/// Шаблон потока - сохраненная конфигурация для повторного использования
/// </summary>
[Table("FlowTemplates")]
public class FlowTemplate : EntityBase
{
    /// <summary>ID учетных данных API</summary>
    [Required]
    public long ApiCredentialId { get; set; }

    /// <summary>Имя шаблона</summary>
    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    /// <summary>Тип шаблона (для десериализации JSON значений)</summary>
    [Required]
    public FlowTemplateType Type { get; set; }

    /// <summary>Описание шаблона</summary>
    [MaxLength(1000)]
    public string? Description { get; set; }

    /// <summary>JSON данные шаблона</summary>
    [Required]
    [Column(TypeName = "text")]
    public string Value { get; set; }

    /// <summary>Дата последнего использования</summary>
    public DateTimeOffset? LastUsedAt { get; set; }

    /// <summary>Количество использований</summary>
    public int UseCount { get; set; } = 0;

    /// <summary>Активен ли шаблон</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>Версия шаблона</summary>
    public int Version { get; set; } = 1;

    /// <summary>Теги для фильтрации</summary>
    [Column(TypeName = "text")]
    public string? Tags { get; set; } // JSON array: ["tag1", "tag2"]

    // Navigation property
    [ForeignKey(nameof(ApiCredentialId))]
    public virtual ApiCredential? ApiCredential { get; set; }
}
