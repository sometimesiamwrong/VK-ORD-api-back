using VkOrdApiWrapper.Entities;

namespace VkOrdApiWrapper.Security
{
    public record TokenPair(string AccessToken, string RefreshToken, DateTime AccessExpiresAt, DateTime RefreshExpiresAt);

    public interface ITokenService
    {
        TokenPair GenerateTokens(User user, string? deviceId = null, string? ip = null);
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
    }
}


