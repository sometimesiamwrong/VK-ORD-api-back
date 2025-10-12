using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Domain.Entities.VkOrd;
using Domain.Extensions;
using System.Linq.Expressions;

namespace Domain.Data;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<ApiCredential> ApiCredentials { get; set; }
    public DbSet<VkOrdLogicalAccount> VkLogicalAccounts { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<DatabaseScript> DatabaseScripts { get; set; }

    // VK ORD  entities
    public DbSet<VkOrdCounterparty> VkOrdCounterparties { get; set; }
    public DbSet<VkOrdContract> VkOrdContracts { get; set; }
    public DbSet<VkOrdCreative> VkOrdCreatives { get; set; }
    public DbSet<VkOrdMedia> VkOrdMedias { get; set; }
    public DbSet<VkOrdStatistics> VkOrdStatistics { get; set; }

    // VK ORD  relation entities
    public DbSet<VkOrdCreativeContract> VkOrdCreativeContract { get; set; }
    public DbSet<VkOrdContractParty> VkOrdContractParties { get; set; }
    public DbSet<VkOrdCreativeMedia> VkOrdCreativeMedia { get; set; }

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

            b.HasOne(a => a.LogicalAccount)
                .WithMany()
                .HasForeignKey(a => a.LogicalAccountId)
                .OnDelete(DeleteBehavior.Cascade);

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
            .Property(a => a.ApiEnvironment)
            .HasMaxLength(20);

        modelBuilder.Entity<DatabaseScript>(b =>
        {
            b.HasIndex(s => s.ScriptName).IsUnique();
        });

        // VkOrdLogicalAccount
        modelBuilder.Entity<VkOrdLogicalAccount>(b =>
        {
            b.ConfigureEntityBase();
            b.HasIndex(la => la.PublicId).IsUnique();
            
            // Связь с VkOrdBase сущностями
            b.HasMany(la => la.VkOrdEntities)
                .WithOne(e => e.OrdLogicalAccount)
                .HasForeignKey(e => e.LogicalAccountId);
        });

        // Исключаем DTO классы и базовые абстрактные классы из модели EF
        modelBuilder.Ignore<Domain.VkOrdApi.Contract.VkOrdApiContractResponse>();
        modelBuilder.Ignore<Domain.VkOrdApi.Contract.VkOrdApiContractListResponse>();
        modelBuilder.Ignore<Domain.VkOrdApi.Creative.VkOrdApiCreativeResponse>();
        modelBuilder.Ignore<Domain.VkOrdApi.Creative.VkOrdApiCreativeV2Response>();
        modelBuilder.Ignore<Domain.VkOrdApi.Creative.VkOrdApiCreativeV3Response>();
        modelBuilder.Ignore<Domain.VkOrdApi.Person.VkOrdApiPersonResponse>();
        modelBuilder.Ignore<Domain.VkOrdApi.Person.VkOrdApiPersonListResponse>();
        modelBuilder.Ignore<Domain.VkOrdApi.Media.VkOrdApiMediaInfoResponse>();
        modelBuilder.Ignore<Domain.VkOrdApi.Invoice.VkOrdApiFullInvoiceResponse>();
        modelBuilder.Ignore<Domain.VkOrdApi.ErirStatus.VkOrdApiErirStatusResponse>();
        modelBuilder.Ignore<VkOrdBase>();

        // VK ORD  entities configuration
        ConfigureVkOrdEntities(modelBuilder);
    }

    private void ConfigureVkOrdEntities(ModelBuilder modelBuilder)
    {
        // VkOrdCounterparty
        modelBuilder.Entity<VkOrdCounterparty>(b =>
        {
            b.ConfigureVkOrdBase();
            b.HasKey(e => e.Id);
            b.Property(e => e.Id).ValueGeneratedOnAdd();

            // Индексы
            b.HasIndex(c => c.SyncStatus);

            // Ограничения
            b.Property(c => c.ExternalId).HasMaxLength(255);
        });

        // VkOrdContract
        modelBuilder.Entity<VkOrdContract>(b =>
        {
            b.ConfigureVkOrdBase();
            b.HasKey(e => e.Id);
            b.Property(e => e.Id).ValueGeneratedOnAdd();

            // Обычные индексы
            b.HasIndex(c => c.SyncStatus);
        });

        // VkOrdCreative
        modelBuilder.Entity<VkOrdCreative>(b =>
        {
            b.ConfigureVkOrdBase();
            b.HasKey(e => e.Id);
            b.Property(e => e.Id).ValueGeneratedOnAdd();

            // Обычные индексы
            b.HasIndex(c => c.SyncStatus);
        });

        // VkOrdMedia
        modelBuilder.Entity<VkOrdMedia>(b =>
        {
            b.ConfigureVkOrdBase();
            b.HasKey(e => e.Id);
            b.Property(e => e.Id).ValueGeneratedOnAdd();

            // Обычные индексы
            b.HasIndex(c => c.SyncStatus);
        });

        // VkOrdStatistics
        modelBuilder.Entity<VkOrdStatistics>(b =>
        {
            b.ConfigureVkOrdBase();
            b.HasKey(e => e.Id);
            b.Property(e => e.Id).ValueGeneratedOnAdd();

            // Связь с креативом
            b.HasOne(c => c.Creative)
                .WithMany(c => c.Statistics)
                .HasForeignKey(c => c.CreativeId)
                .OnDelete(DeleteBehavior.Cascade);

            // Индексы
            b.HasIndex(c => c.CreativeExternalId);
            b.HasIndex(c => c.PadExternalId);
            b.HasIndex(c => c.Period);
            b.HasIndex(c => c.StatisticsType);
            b.HasIndex(c => c.DateStartPlanned);
            b.HasIndex(c => c.DateEndPlanned);
            b.HasIndex(c => c.DateStartActual);
            b.HasIndex(c => c.DateEndActual);
            b.HasIndex(c => c.SyncStatus);

            // Ограничения
            b.Property(c => c.ExternalId).HasMaxLength(255);
            b.Property(c => c.CreativeExternalId).HasMaxLength(255);
            b.Property(c => c.PadExternalId).HasMaxLength(255);
            b.Property(c => c.Period).HasMaxLength(7);
            b.Property(c => c.StatisticsType).HasMaxLength(50);
            b.Property(c => c.PayType).HasMaxLength(50);
            b.Property(c => c.AmountPerEvent).HasPrecision(18, 8);
        });

        // VkOrdContractParty
        modelBuilder.Entity<VkOrdContractParty>(b =>
        {
            b.HasKey(cp => cp.Id);

            // Составной ключ
            b.HasIndex(cp => new { cp.ContractId, cp.CounterpartyId, cp.Role });

            // Связь с договором (используем Id как внешний ключ)
            b.HasOne(cp => cp.Contract)
                .WithMany(c => c.ContractParties)
                .HasForeignKey(cp => cp.ContractId)
                .OnDelete(DeleteBehavior.Cascade);

            // Связь с контрагентом (используем Id как внешний ключ)
            b.HasOne(cp => cp.Counterparty)
                .WithMany(c => c.ContractParties)
                .HasForeignKey(cp => cp.CounterpartyId)
                .OnDelete(DeleteBehavior.Cascade);

            // Индексы
            b.HasIndex(cp => cp.ContractId);
            b.HasIndex(cp => cp.CounterpartyId);
            b.HasIndex(cp => cp.Role);
            b.HasIndex(cp => cp.CreatedAt);

            // Ограничения
            b.Property(cp => cp.Role).HasMaxLength(50);
        });

        // VkOrdCreativeMedia
        modelBuilder.Entity<VkOrdCreativeMedia>(b =>
        {
            // Составной ключ
            b.HasKey(cm => new { cm.CreativeId, cm.MediaId });

            // Связь с креативом
            b.HasOne(cm => cm.Creative)
                .WithMany(c => c.CreativeMedia)
                .HasForeignKey(cm => cm.CreativeId)
                .OnDelete(DeleteBehavior.Cascade);

            // Связь с медиа
            b.HasOne(cm => cm.Media)
                .WithMany(m => m.CreativeMedia)
                .HasForeignKey(cm => cm.MediaId)
                .OnDelete(DeleteBehavior.Cascade);

            // Индексы
            b.HasIndex(cm => cm.CreativeId);
            b.HasIndex(cm => cm.MediaId);
            b.HasIndex(cm => cm.Order);
            b.HasIndex(cm => cm.CreatedAt);
        });

        // VkOrdCreativeContract
        modelBuilder.Entity<VkOrdCreativeContract>(b =>
        {
            b.HasKey(cc => new { cc.CreativeId, cc.ContractId });

            // Связь с креативом
            b.HasOne(cc => cc.Creative)
                .WithMany(c => c.CreativeContracts)
                .HasForeignKey(cc => cc.CreativeId)
                .OnDelete(DeleteBehavior.Cascade);

            // Связь с договором
            b.HasOne(cc => cc.Contract)
                .WithMany(c => c.CreativeContracts)
                .HasForeignKey(cc => cc.ContractId)
                .OnDelete(DeleteBehavior.Cascade);

            // Индексы
            b.HasIndex(cc => cc.CreativeId);
            b.HasIndex(cc => cc.ContractId);
            b.HasIndex(cc => cc.CreatedAt);
        });
    }

    public static Expression<Func<T, bool>> DefaultGetVkOrd<T>(string externalId, ApiCredential apiCredential)
        where T : VkOrdBase
    {
        return t => t.LogicalAccountId == apiCredential.LogicalAccountId && t.ExternalId == externalId && t.IsDeleted == false;
    }

    public static Expression<Func<T, bool>> DefaultGetVkOrd<T>(long entityId, ApiCredential apiCredential)
        where T : VkOrdBase
    {
        return t => t.LogicalAccountId == apiCredential.LogicalAccountId && t.Id == entityId && t.IsDeleted == false;
    }
}



