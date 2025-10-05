using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Domain.ValueGeneration;
using Domain.Extensions;

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

        // Настраиваем наследование - каждая сущность имеет свою таблицу (TPT)
        // EntityBase поля наследуются в каждую дочернюю сущность

        modelBuilder.Entity<User>(b =>
        {
            b.ConfigureEntityBase();
            b.HasIndex(u => u.UserName).IsUnique();
        });

        modelBuilder.Entity<ApiCredential>(b =>
        {
            b.ConfigureEntityBase();
            b.HasOne(a => a.User)
                .WithMany(u => u.ApiCredentials)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<RefreshToken>(b =>
        {
            b.ConfigureEntityBase();
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



