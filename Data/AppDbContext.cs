using Microsoft.EntityFrameworkCore;
using VkOrdApiWrapper.Models.Entities;

namespace VkOrdApiWrapper.Data
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
		{
		}

		public DbSet<ContractEntity> Contracts { get; set; }
		public DbSet<CreativeEntity> Creatives { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<ContractEntity>(builder =>
			{
				builder.HasKey(x => x.Id);
				builder.Property(x => x.ExternalId).IsRequired();
				builder.HasIndex(x => x.ExternalId).IsUnique(false);
			});

			modelBuilder.Entity<CreativeEntity>(builder =>
			{
				builder.HasKey(x => x.Id);
				builder.Property(x => x.ExternalId).IsRequired();
				builder.HasIndex(x => x.ExternalId).IsUnique(false);

				// JSON conversions for lists to store in database
				builder.Property(x => x.ContractExternalIds)
					.HasConversion(
						v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions)null),
						v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions)null) ?? new List<string>());

				builder.Property(x => x.KKTYCodes)
					.HasConversion(
						v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions)null),
						v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions)null) ?? new List<string>());

				builder.Property(x => x.ContentUrls)
					.HasConversion(
						v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions)null),
						v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions)null) ?? new List<string>());
			});
		}
	}
}

