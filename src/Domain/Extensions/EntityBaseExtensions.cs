using Domain.Entities;
using Domain.ValueGeneration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Domain.Extensions;

/// <summary>
/// Методы расширения для настройки EntityBase в Entity Framework
/// </summary>
public static class EntityBaseExtensions
{
    /// <summary>
    /// Настраивает базовые поля EntityBase для сущности
    /// </summary>
    /// <typeparam name="TEntity">Тип сущности, наследующей EntityBase</typeparam>
    /// <param name="entityBuilder">Строитель сущности</param>
    /// <returns>Строитель сущности с настроенными базовыми полями</returns>
    public static EntityTypeBuilder<TEntity> ConfigureEntityBase<TEntity>(this EntityTypeBuilder<TEntity> entityBuilder)
        where TEntity : EntityBase
    {
        entityBuilder.HasKey(e => e.Id);
        entityBuilder.Property(e => e.Id).ValueGeneratedOnAdd();

        entityBuilder.Property(e => e.PublicId)
            .HasColumnType("uuid");
        entityBuilder.HasIndex(e => e.PublicId)
            .IsUnique();

        // Клиентская генерация последовательного Guid (v7)
        entityBuilder.Property(e => e.PublicId)
            .HasValueGenerator<NpgsqlSequentialGuidValueGenerator>();

        // Таймстемпы
        entityBuilder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp with time zone");
        entityBuilder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp with time zone");

        entityBuilder.Property(e => e.RowVersion)
            .HasDefaultValue(1)
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken()
            .HasColumnType("integer");

        entityBuilder.Property(e => e.IsDeleted)
            .HasDefaultValue(false)
            .HasColumnType("boolean");

        return entityBuilder;
    }
}
