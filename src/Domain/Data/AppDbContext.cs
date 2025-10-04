using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Domain.ValueGeneration;

namespace Domain.Data;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<ApiCredential> ApiCredentials { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<DatabaseScript> DatabaseScripts { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<EntityBase>(b =>
        {
            b.HasKey(e => e.Id);
            b.Property(e => e.Id).ValueGeneratedOnAdd();

            b.Property(e => e.PublicId)
                .HasColumnType("uuid");
        b.HasIndex(e => e.PublicId)
            .IsUnique();

        // Клиентская генерация последовательного Guid (v7)
        b.Property(e => e.PublicId)
            .HasValueGenerator<NpgsqlSequentialGuidValueGenerator>();

        // Таймстемпы
        b.Property(e => e.CreatedAt)
            .HasColumnType("timestamp with time zone");
        b.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp with time zone");

        b.Property(e => e.RowVersion)
            .HasDefaultValue(1)
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken()
            .HasColumnType("integer");

        b.Property(e => e.IsDeleted)
            .HasDefaultValue(false)
            .HasColumnType("boolean");
        });

        modelBuilder.Entity<User>(b =>
        {
            b.HasIndex(u => u.UserName).IsUnique();

            b.Property(u => u.RowVersion)
            .IsConcurrencyToken()
            .HasColumnType("integer");
        });

        modelBuilder.Entity<ApiCredential>(b =>
        {
            b.HasOne(a => a.User)
                .WithMany(u => u.ApiCredentials)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            b.Property(a => a.RowVersion)
                .IsConcurrencyToken()
                .HasColumnType("integer");

            b.Property(a => a.Environment)
                .HasMaxLength(20);
        });

        modelBuilder.Entity<RefreshToken>(b =>
        {
            b.HasOne(r => r.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ApiCredential>()
            .Property(a => a.Environment)
            .HasMaxLength(20);

        modelBuilder.Entity<DatabaseScript>(b =>
        {
            b.HasIndex(s => s.ScriptName).IsUnique();
        });
    }   
}



