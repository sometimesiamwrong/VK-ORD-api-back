using Domain.Data;
using Domain.Entities;
using WebApp.Repositories.Interfaces.ApiCredentials;
using WebApp.Security;

namespace WebApp.Repositories.Implementation.ApiCredentials
{
    /// <summary>
    /// Репозиторий для сохранения ApiCredential
    /// </summary>
    public class SaveApiCredentialRepository : ISaveApiCredentialRepository
    {
        private readonly AppDbContext _db;
        private readonly ISecretProtector _protector;

        public SaveApiCredentialRepository(AppDbContext db, ISecretProtector protector)
        {
            _db = db;
            _protector = protector;
        }

        public async Task<ApiCredential?> Save(ApiCredential credential, CancellationToken cancellationToken)
        {
            // Шифруем токен всегда
            var encryptedToken = _protector.Encrypt(credential.TokenEncrypted);
            credential.TokenEncrypted = encryptedToken;

            if (credential.IsNewOrUpdate())
            {
                // Создание новой сущности
                _db.ApiCredentials.Add(credential);
                await _db.SaveChangesAsync();
                return credential;
            }
            else
            {
                // Обновление существующей сущности
                var existing = await _db.ApiCredentials.FindAsync(credential.Id);
                if (existing == null)
                    return null;

                existing.Environment = credential.Environment;
                existing.TokenEncrypted = encryptedToken;
                existing.DisplayName = credential.DisplayName;
                existing.UpdatedAt = DateTimeOffset.UtcNow;

                await _db.SaveChangesAsync();
                return existing;
            }
        }
    }
}
