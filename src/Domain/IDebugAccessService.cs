using Domain.Entities;

namespace WebApp.Security
{
    public interface IDebugAccessService
    {
        Task<string> GenerateImpersonateLink(long userId, string? createdByIp = null);

        Task<string> GenerateDecryptLink(Guid apiCredentialGuid, string? createdByIp = null);

        Task<DebugAccessToken> ValidateAndConsumeToken(string token, string? usedByIp = null);

        Task CleanupExpiredTokens();
    }
}
