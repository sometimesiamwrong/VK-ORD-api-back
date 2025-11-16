using Domain.Data;
using Domain.Entities;
using Domain.Entities.VkOrd;
using Domain.Repositories.Interfaces.ApiCredentials;
using Microsoft.EntityFrameworkCore;
using WebApp.Security;

namespace Domain.Repositories.Implementations.ApiCredentials
{
    /// <summary>
    /// Репозиторий для сохранения ApiCredential
    /// </summary>
    public class SaveApiCredentialRepository : ISaveApiCredentialRepository
    {
        private readonly Func<AppDbContext> _contextFactory;
        private readonly ISecretProtector _protector;

        public SaveApiCredentialRepository(Func<AppDbContext> contextFactory, ISecretProtector protector)
        {
            _contextFactory = contextFactory;
            _protector = protector;
        }

        public async Task<ApiCredential?> Save(ApiCredential credential, CancellationToken cancellationToken)
        {
            await using var context = _contextFactory();
            // Шифруем токен всегда
            var encryptedToken = _protector.Encrypt(credential.TokenEncrypted);
            credential.TokenEncrypted = encryptedToken;
            
            var dublicate = await context.ApiCredentials
                .Include(x=>x.LogicalAccount)
                .FirstOrDefaultAsync(x=>x.TokenEncrypted == encryptedToken, cancellationToken);

            VkOrdLogicalAccount logical;

            if (dublicate == null)
            {
                var newLogical = new VkOrdLogicalAccount()
                {
                    CreatedAt = DateTimeOffset.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                };
                logical = (await context.VkLogicalAccounts.AddAsync(newLogical, cancellationToken)).Entity;
            } else {
                logical = dublicate.LogicalAccount;
            }

            if (credential.IsNewOrUpdate())
            {
                // Создание новой сущности
                context.ApiCredentials.Add(credential);
                credential.LogicalAccount = logical;
                await context.SaveChangesAsync();
                return credential;
            }
            else
            {
                // Обновление существующей сущности
                var existing = await context.ApiCredentials.FindAsync(credential.Id);
                if (existing == null)
                    return null;

                existing.ApiEnvironment = credential.ApiEnvironment;
                existing.TokenEncrypted = encryptedToken;
                existing.DisplayName = credential.DisplayName;
                existing.UpdatedAt = DateTimeOffset.UtcNow;

                await context.SaveChangesAsync();
                return existing;
            }
        }
    }
}
