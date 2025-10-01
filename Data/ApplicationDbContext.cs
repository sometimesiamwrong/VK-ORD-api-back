using Microsoft.EntityFrameworkCore;
using VkOrdApiWrapper.Entities;

namespace VkOrdApiWrapper.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<ApiCredential> ApiCredentials => Set<ApiCredential>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<DatabaseScript> DatabaseScripts => Set<DatabaseScript>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.UserName)
                .IsUnique();

            // Настройка RowVersion для PostgreSQL (integer с триггерами)
            modelBuilder.Entity<User>()
                .Property(u => u.RowVersion)
                .HasDefaultValue(1)
                .ValueGeneratedOnAddOrUpdate()
                .IsConcurrencyToken()
                .HasColumnType("integer");

            modelBuilder.Entity<ApiCredential>()
                .HasOne(a => a.User)
                .WithMany(u => u.ApiCredentials)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ApiCredential>()
                .Property(a => a.RowVersion)
                .HasDefaultValue(1)
                .ValueGeneratedOnAddOrUpdate()
                .IsConcurrencyToken()
                .HasColumnType("integer");

            modelBuilder.Entity<RefreshToken>()
                .HasOne(r => r.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ApiCredential>()
                .Property(a => a.Environment)
                .HasMaxLength(20);

            modelBuilder.Entity<DatabaseScript>()
                .HasIndex(s => s.ScriptName)
                .IsUnique();
        }
    }
}


