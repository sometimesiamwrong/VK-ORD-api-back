using System.Security.Cryptography;
using Domain.Data;
using Domain.Entities;
using Domain.Entities.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebApp.Configuration;

namespace WebApp.Security
{
    public class DebugAccessService : IDebugAccessService
    {
        private readonly Func<AppDbContext> _context;
        private readonly DebugSettings _settings;
        private readonly ILogger<DebugAccessService> _logger;

        public DebugAccessService(
            Func<AppDbContext> context,
            IOptions<DebugSettings> settings,
            ILogger<DebugAccessService> logger)
        {
            _context = context;
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task<string> GenerateImpersonateLink(long userId, string? createdByIp = null)
        {
            await using var context = _context();
            var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);
            if (user == null)
            {
                throw new InvalidOperationException($"User with ID {userId} not found");
            }

            var token = GenerateSecureToken();
            var debugToken = new DebugAccessToken
            {
                Token = token,
                Purpose = DebugAccessPurpose.Impersonate,
                UserId = userId,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_settings.TokenExpiryMinutes),
                CreatedByIp = createdByIp,
                PublicId = Guid.NewGuid(),
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            };

            context.DebugAccessTokens.Add(debugToken);
            await context.SaveChangesAsync();

            _logger.LogInformation("Generated impersonate link for user {UserId} from IP {IP}", userId, createdByIp);

            return $"{_settings.BaseUrl}/#/auth/verify?token={token}";
        }

        public async Task<string> GenerateDecryptLink(Guid apiCredentialGuid, string? createdByIp = null)
        {
            await using var context = _context();
            var credential = await context.ApiCredentials
                .FirstOrDefaultAsync(c => c.PublicId == apiCredentialGuid && !c.IsDeleted);

            if (credential == null)
            {
                throw new InvalidOperationException($"API Credential with GUID {apiCredentialGuid} not found");
            }

            var token = GenerateSecureToken();
            var debugToken = new DebugAccessToken
            {
                Token = token,
                Purpose = DebugAccessPurpose.DecryptApiKey,
                ApiCredentialId = credential.Id,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_settings.TokenExpiryMinutes),
                CreatedByIp = createdByIp,
                PublicId = Guid.NewGuid(),
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            };

            context.DebugAccessTokens.Add(debugToken);
            await context.SaveChangesAsync();

            _logger.LogInformation("Generated decrypt link for credential {CredentialGuid} from IP {IP}",
                apiCredentialGuid, createdByIp);

            return $"{_settings.BaseUrl}/api/debug/v1/decrypt-credential?token={token}";
        }

        public async Task<DebugAccessToken> ValidateAndConsumeToken(string token, string? usedByIp = null)
        {
            await using var context = _context();
            var debugToken = await context.DebugAccessTokens
                .Include(d => d.User)
                .Include(d => d.ApiCredential)
                .FirstOrDefaultAsync(d => d.Token == token && !d.IsDeleted);

            if (debugToken == null)
            {
                _logger.LogWarning("Invalid debug token attempted from IP {IP}", usedByIp);
                throw new InvalidOperationException("Invalid or expired token");
            }

            if (!debugToken.IsValid)
            {
                _logger.LogWarning("Expired or used debug token attempted from IP {IP}. Token created at {CreatedAt}, expires at {ExpiresAt}, IsUsed: {IsUsed}",
                    usedByIp, debugToken.CreatedAt, debugToken.ExpiresAt, debugToken.IsUsed);
                throw new InvalidOperationException("Token has already been used or has expired");
            }

            debugToken.IsUsed = true;
            debugToken.UsedAt = DateTime.UtcNow;
            debugToken.UsedByIp = usedByIp;
            debugToken.UpdatedAt = DateTimeOffset.UtcNow;

            await context.SaveChangesAsync();

            _logger.LogInformation("Debug token consumed successfully. Purpose: {Purpose}, IP: {IP}",
                debugToken.Purpose, usedByIp);

            return debugToken;
        }

        public async Task CleanupExpiredTokens()
        {
            await using var context = _context();
            var cutoffDate = DateTime.UtcNow.AddDays(-7);

            var expiredTokens = await context.DebugAccessTokens
                .Where(d => (d.IsUsed || d.ExpiresAt < cutoffDate) && !d.IsDeleted)
                .ToListAsync();

            foreach (var token in expiredTokens)
            {
                token.IsDeleted = true;
                token.UpdatedAt = DateTimeOffset.UtcNow;
            }

            await context.SaveChangesAsync();

            _logger.LogInformation("Cleaned up {Count} expired debug access tokens", expiredTokens.Count);
        }

        private static string GenerateSecureToken()
        {
            var bytes = RandomNumberGenerator.GetBytes(32);
            return Convert.ToBase64String(bytes)
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "");
        }
    }
}
