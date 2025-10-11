using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Domain.Entities.VkOrdCache;
using Domain.ValueGeneration;
using Domain.Extensions;

namespace Domain.Data;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<ApiCredential> ApiCredentials { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<DatabaseScript> DatabaseScripts { get; set; }

    // VK ORD Cache entities
    public DbSet<VkOrdCounterpartyCache> VkOrdCounterpartyCache { get; set; }
    public DbSet<VkOrdContractCache> VkOrdContractCache { get; set; }
    public DbSet<VkOrdCreativeCache> VkOrdCreativeCache { get; set; }
    public DbSet<VkOrdMediaCache> VkOrdMediaCache { get; set; }
    public DbSet<VkOrdStatisticsCache> VkOrdStatisticsCache { get; set; }

    // VK ORD Cache relation entities
    public DbSet<VkOrdContractParty> VkOrdContractParty { get; set; }
    public DbSet<VkOrdCounterpartyRelation> VkOrdCounterpartyRelation { get; set; }
    public DbSet<VkOrdCreativeContract> VkOrdCreativeContract { get; set; }
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

        // VK ORD Cache entities configuration
        ConfigureVkOrdCacheEntities(modelBuilder);
    }

    private void ConfigureVkOrdCacheEntities(ModelBuilder modelBuilder)
    {
        // VkOrdCounterpartyCache
        modelBuilder.Entity<VkOrdCounterpartyCache>(b =>
        {
            // Первичный ключ - автоинкрементный Id
            b.HasKey(c => c.Id);
            
            // Уникальный составной индекс
            b.HasIndex(c => new { c.ApiCredentialId, c.ExternalId }).IsUnique();
            
            // Связь с ApiCredential
            b.HasOne(c => c.ApiCredential)
                .WithMany()
                .HasForeignKey(c => c.ApiCredentialId);

            // Индексы
            b.HasIndex(c => c.ApiCredentialId);
            b.HasIndex(c => c.Inn);
            b.HasIndex(c => c.Name);
            b.HasIndex(c => c.CachedAt);
            b.HasIndex(c => c.ExpiresAt);
            b.HasIndex(c => c.SyncStatus);

            // Ограничения
            b.Property(c => c.ExternalId).HasMaxLength(255);
            b.Property(c => c.Inn).HasMaxLength(12);
            b.Property(c => c.Name).HasMaxLength(500);
            b.Property(c => c.RsUrl).HasMaxLength(500);
        });

        // VkOrdContractCache
        modelBuilder.Entity<VkOrdContractCache>(b =>
        {
            // Первичный ключ - автоинкрементный Id
            b.HasKey(c => c.Id);
            
            // Уникальный составной индекс
            b.HasIndex(c => new { c.ApiCredentialId, c.ExternalId }).IsUnique();
            
            // Связь с ApiCredential
            b.HasOne(c => c.ApiCredential)
                .WithMany()
                .HasForeignKey(c => c.ApiCredentialId);

            // Связь с родительским договором
            b.HasOne(c => c.ParentContract)
                .WithMany(c => c.AdditionalContracts)
                .HasForeignKey(c => c.ParentContractId)
                .OnDelete(DeleteBehavior.Restrict);

            // Индексы
            b.HasIndex(c => c.ApiCredentialId);
            b.HasIndex(c => c.ClientExternalId);
            b.HasIndex(c => c.ContractorExternalId);
            b.HasIndex(c => c.Date);
            b.HasIndex(c => c.DateEnd);
            b.HasIndex(c => c.CachedAt);
            b.HasIndex(c => c.ExpiresAt);
            b.HasIndex(c => c.SyncStatus);

            // Ограничения
            b.Property(c => c.ExternalId).HasMaxLength(255);
            b.Property(c => c.ClientExternalId).HasMaxLength(255);
            b.Property(c => c.ContractorExternalId).HasMaxLength(255);
            b.Property(c => c.Type).HasMaxLength(50);
            b.Property(c => c.ActionType).HasMaxLength(50);
            b.Property(c => c.SubjectType).HasMaxLength(50);
            b.Property(c => c.Serial).HasMaxLength(255);
            b.Property(c => c.ParentContractExternalId).HasMaxLength(255);
            b.Property(c => c.Cid).HasMaxLength(255);
            b.Property(c => c.Amount).HasPrecision(18, 2);
        });

        // VkOrdCreativeCache
        modelBuilder.Entity<VkOrdCreativeCache>(b =>
        {
            // Первичный ключ - автоинкрементный Id
            b.HasKey(c => c.Id);
            
            // Уникальный составной индекс
            b.HasIndex(c => new { c.ApiCredentialId, c.ExternalId }).IsUnique();
            
            // Связь с ApiCredential
            b.HasOne(c => c.ApiCredential)
                .WithMany()
                .HasForeignKey(c => c.ApiCredentialId);

            // Индексы
            b.HasIndex(c => c.ApiCredentialId);
            b.HasIndex(c => c.Erid);
            b.HasIndex(c => c.PersonExternalId);
            b.HasIndex(c => c.Name);
            b.HasIndex(c => c.Status);
            b.HasIndex(c => c.CachedAt);
            b.HasIndex(c => c.ExpiresAt);
            b.HasIndex(c => c.SyncStatus);

            // Ограничения
            b.Property(c => c.ExternalId).HasMaxLength(255);
            b.Property(c => c.Erid).HasMaxLength(255);
            b.Property(c => c.PersonExternalId).HasMaxLength(255);
            b.Property(c => c.Name).HasMaxLength(255);
            b.Property(c => c.Brand).HasMaxLength(255);
            b.Property(c => c.Category).HasMaxLength(255);
            b.Property(c => c.Description).HasMaxLength(2000);
            b.Property(c => c.PayType).HasMaxLength(50);
            b.Property(c => c.Form).HasMaxLength(50);
            b.Property(c => c.Status).HasMaxLength(50);
        });

        // VkOrdMediaCache
        modelBuilder.Entity<VkOrdMediaCache>(b =>
        {
            // Первичный ключ - автоинкрементный Id
            b.HasKey(c => c.Id);
            
            // Уникальный составной индекс
            b.HasIndex(c => new { c.ApiCredentialId, c.ExternalId }).IsUnique();
            
            // Связь с ApiCredential
            b.HasOne(c => c.ApiCredential)
                .WithMany()
                .HasForeignKey(c => c.ApiCredentialId);

            // Индексы
            b.HasIndex(c => c.ApiCredentialId);
            b.HasIndex(c => c.Sha256);
            b.HasIndex(c => c.Filename);
            b.HasIndex(c => c.ContentType);
            b.HasIndex(c => c.MediaType);
            b.HasIndex(c => c.UploadStatus);
            b.HasIndex(c => c.CachedAt);
            b.HasIndex(c => c.ExpiresAt);
            b.HasIndex(c => c.SyncStatus);

            // Ограничения
            b.Property(c => c.ExternalId).HasMaxLength(255);
            b.Property(c => c.Filename).HasMaxLength(500);
            b.Property(c => c.Sha256).HasMaxLength(64);
            b.Property(c => c.ContentType).HasMaxLength(100);
            b.Property(c => c.Description).HasMaxLength(1000);
            b.Property(c => c.MediaType).HasMaxLength(50);
            b.Property(c => c.DownloadUrl).HasMaxLength(1000);
            b.Property(c => c.UploadStatus).HasMaxLength(50);
        });

        // VkOrdStatisticsCache
        modelBuilder.Entity<VkOrdStatisticsCache>(b =>
        {
            // Первичный ключ - автоинкрементный Id
            b.HasKey(c => c.Id);
            
            // Уникальный составной индекс
            b.HasIndex(c => new { c.ApiCredentialId, c.ExternalId }).IsUnique();
            
            // Связь с ApiCredential
            b.HasOne(c => c.ApiCredential)
                .WithMany()
                .HasForeignKey(c => c.ApiCredentialId);

            // Связь с креативом
            b.HasOne(c => c.Creative)
                .WithMany(c => c.Statistics)
                .HasForeignKey(c => c.CreativeId)
                .OnDelete(DeleteBehavior.Cascade);

            // Индексы
            b.HasIndex(c => c.ApiCredentialId);
            b.HasIndex(c => c.CreativeExternalId);
            b.HasIndex(c => c.PadExternalId);
            b.HasIndex(c => c.Period);
            b.HasIndex(c => c.StatisticsType);
            b.HasIndex(c => c.DateStartPlanned);
            b.HasIndex(c => c.DateEndPlanned);
            b.HasIndex(c => c.DateStartActual);
            b.HasIndex(c => c.DateEndActual);
            b.HasIndex(c => c.CachedAt);
            b.HasIndex(c => c.ExpiresAt);
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
            // Составной ключ
            b.HasKey(cp => new { cp.ContractId, cp.CounterpartyId, cp.Role });
            
            // Связь с договором (используем Id как внешний ключ)
            b.HasOne(cp => cp.Contract)
                .WithMany(c => c.ContractParties)
                .HasForeignKey(cp => cp.ContractId)
                .OnDelete(DeleteBehavior.Cascade);

            // Связь с контрагентом (используем Id как внешний ключ)
            b.HasOne(cp => cp.Counterparty)
                .WithMany(c => c.ClientContracts)
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

        // VkOrdCounterpartyRelation
        modelBuilder.Entity<VkOrdCounterpartyRelation>(b =>
        {
            // Составной ключ
            b.HasKey(cr => new { cr.FromCounterpartyId, cr.ToCounterpartyId, cr.RelationType });
            
            // Связь с первым контрагентом
            b.HasOne(cr => cr.FromCounterparty)
                .WithMany(c => c.Relations)
                .HasForeignKey(cr => cr.FromCounterpartyId)
                .OnDelete(DeleteBehavior.Cascade);

            // Связь со вторым контрагентом
            b.HasOne(cr => cr.ToCounterparty)
                .WithMany()
                .HasForeignKey(cr => cr.ToCounterpartyId)
                .OnDelete(DeleteBehavior.Cascade);

            // Индексы
            b.HasIndex(cr => cr.FromCounterpartyId);
            b.HasIndex(cr => cr.ToCounterpartyId);
            b.HasIndex(cr => cr.RelationType);
            b.HasIndex(cr => cr.CreatedAt);

            // Ограничения
            b.Property(cr => cr.RelationType).HasMaxLength(50);
            b.Property(cr => cr.Description).HasMaxLength(500);
        });

        // VkOrdCreativeContract
        modelBuilder.Entity<VkOrdCreativeContract>(b =>
        {
            // Составной ключ
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
    }
}



