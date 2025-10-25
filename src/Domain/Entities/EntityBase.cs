using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Entities;

/// <summary>
/// Базовая сущность
/// </summary>
public abstract class EntityBase
{
    /// <summary>
    /// Идентификатор сущности
    /// </summary>
    [Key]
    public long Id { get; set; }

    /// <summary>
    /// Публичный идентификатор сущности
    /// </summary>
    public Guid PublicId { get; set; }  

    /// <summary>
    /// Дата создания
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Дата обновления
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }

    /// <summary>
    /// Версия строки
    /// </summary>
    [JsonIgnore]
    public int RowVersion { get; set; }

    /// <summary>
    /// Удалена ли сущность
    /// </summary>
    [JsonIgnore]
    public bool IsDeleted { get; set; }

    /// <returns>true если новая сущность (Id == 0), false если существующий</returns>
    public bool IsNewOrUpdate()
    {
        return Id == 0;
    }
}
