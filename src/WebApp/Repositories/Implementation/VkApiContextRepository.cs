using Domain.Data;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using WebApp.Repositories.Interfaces;
using WebApp.Security;

namespace WebApp.Repositories.Implementation
{
    /// <summary>
    /// Репозиторий для получения контекста VK API
    /// </summary>
    public class VkApiContextRepository : IVkApiContextRepository
    {
        private readonly AppDbContext _db;
        private readonly ISecretProtector _protector;

        public VkApiContextRepository(AppDbContext db, ISecretProtector protector)
        {
            _db = db;
            _protector = protector;
        }

        public async Task<VkApiContext?> GetVkApiContextAsync(long id, long userId)
        {
            // Verify that the credential belongs to the user
            var cred = await _db.ApiCredentials
                .Where(c => c.Id == id && c.UserId == userId)
                .FirstOrDefaultAsync();

            if (cred == null)
            {
                return null;
            }

            var token = _protector.Decrypt(cred.TokenEncrypted);

            return new VkApiContext 
            {
                ApiKey = token, 
                Route = cred.Environment 
            };
        }
    }
}
