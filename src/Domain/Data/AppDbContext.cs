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
    public DbSet<FlowTemplate> FlowTemplates { get; set; }

    // VK ORD  entities
    public DbSet<VkOrdCounterparty> VkOrdCounterparties { get; set; }
    public DbSet<VkOrdContract> VkOrdContracts { get; set; }
    public DbSet<VkOrdCreative> VkOrdCreatives { get; set; }
    public DbSet<VkOrdMedia> VkOrdMedias { get; set; }
    public DbSet<VkOrdStatistic> VkOrdStatistics { get; set; }
    public DbSet<VkOrdInvoice> VkOrdInvoices { get; set; }
    public DbSet<VkOrdErirStatus> VkOrdErirStatuses { get; set; }

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

        // FlowTemplate
        modelBuilder.Entity<FlowTemplate>(b =>
        {
            b.ConfigureEntityBase();

            b.HasOne(f => f.ApiCredential)
                .WithMany(a => a.FlowTemplates)
                .HasForeignKey(f => f.ApiCredentialId)
                .OnDelete(DeleteBehavior.Cascade);

            // Индексы
            b.HasIndex(f => f.ApiCredentialId).HasFilter("\"IsDeleted\" = false");
            b.HasIndex(f => new { f.ApiCredentialId, f.IsActive }).HasFilter("\"IsDeleted\" = false");
            b.HasIndex(f => f.Name).HasFilter("\"IsDeleted\" = false");
            b.HasIndex(f => f.Type).HasFilter("\"IsDeleted\" = false");
            b.HasIndex(f => f.CreatedAt).HasFilter("\"IsDeleted\" = false");
            b.HasIndex(f => f.UseCount).HasFilter("\"IsDeleted\" = false");

            // Unique constraint: одни учетные данные API не могут иметь два активных шаблона с одинаковым именем
            b.HasIndex(f => new { f.ApiCredentialId, f.Name })
                .IsUnique()
                .HasFilter("\"IsDeleted\" = false");
        });

        // VkOrdErirStatus
        modelBuilder.Entity<VkOrdErirStatus>(b =>
        {
            b.ConfigureEntityBase();

            // Уникальный индекс для быстрого поиска и предотвращения дублей
            b.HasIndex(e => new { e.LogicalAccountId, e.ExternalId, e.EntityType })
                .IsUnique()
                .HasFilter("\"IsDeleted\" = false");

            // Индексы для запросов
            b.HasIndex(e => e.LogicalAccountId).HasFilter("\"IsDeleted\" = false");
            b.HasIndex(e => e.EntityType).HasFilter("\"IsDeleted\" = false");
            b.HasIndex(e => e.ErirStatus).HasFilter("\"IsDeleted\" = false");
            b.HasIndex(e => e.UpdatedByUserTs).HasFilter("\"IsDeleted\" = false");
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

            // Ограничения
            b.Property(c => c.ExternalId).HasMaxLength(255);
        });

        // VkOrdContract
        modelBuilder.Entity<VkOrdContract>(b =>
        {
            b.ConfigureVkOrdBase();
            b.HasKey(e => e.Id);
            b.Property(e => e.Id).ValueGeneratedOnAdd();
        });

        // VkOrdCreative
        modelBuilder.Entity<VkOrdCreative>(b =>
        {
            b.ConfigureVkOrdBase();
            b.HasKey(e => e.Id);
            b.Property(e => e.Id).ValueGeneratedOnAdd();
        });

        // VkOrdMedia
        modelBuilder.Entity<VkOrdMedia>(b =>
        {
            b.ConfigureVkOrdBase();
            b.HasKey(e => e.Id);
            b.Property(e => e.Id).ValueGeneratedOnAdd();
        });

        // VkOrdStatistic
        modelBuilder.Entity<VkOrdStatistic>(b =>
        {
            b.ConfigureVkOrdBase();
            b.HasKey(e => e.Id);
            b.Property(e => e.Id).ValueGeneratedOnAdd();

            // Связь с креативом
            b.HasOne(c => c.Creative)
                .WithMany(c => c.Statistics)
                .HasForeignKey(c => c.CreativeId)
                .OnDelete(DeleteBehavior.Cascade);

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

            // Составной индекс (покрывает ContractId, ContractId+CounterpartyId, ContractId+CounterpartyId+Role)
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

            // Дополнительный индекс для обратного поиска по контрагенту
            b.HasIndex(cp => cp.CounterpartyId);

            // Ограничения
            b.Property(cp => cp.Role).HasMaxLength(50);
        });

        // VkOrdCreativeMedia
        modelBuilder.Entity<VkOrdCreativeMedia>(b =>
        {
            // Составной ключ (PK уже покрывает CreativeId)
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

            // Дополнительный индекс для обратного поиска по медиа
            b.HasIndex(cm => cm.MediaId);
        });

        // VkOrdCreativeContract
        modelBuilder.Entity<VkOrdCreativeContract>(b =>
        {
            // Составной ключ (PK уже покрывает CreativeId)
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

            // Дополнительный индекс для обратного поиска по договору
            b.HasIndex(cc => cc.ContractId);
        });

        // VkOrdInvoice
        modelBuilder.Entity<VkOrdInvoice>(b =>
        {
            b.ConfigureVkOrdBase();
            b.HasKey(e => e.Id);
            b.Property(e => e.Id).ValueGeneratedOnAdd();

            // Ограничения
            b.Property(i => i.ContractExternalId).HasMaxLength(255);

            // Связь с основным договором (опциональная)
            // FK индекс создается автоматически EF Core
            b.HasOne(i => i.Contract)
                .WithMany()
                .HasForeignKey(i => new { i.LogicalAccountId, i.ContractExternalId })
                .HasPrincipalKey(c => new { c.LogicalAccountId, c.ExternalId })
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);
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



