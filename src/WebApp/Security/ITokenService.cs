using Domain.Entities;

namespace WebApp.Security
{
    /// <summary>
    /// Пара токенов
    /// </summary>
    public record TokenPair(string AccessToken, string RefreshToken, DateTime AccessExpiresAt, DateTime RefreshExpiresAt);

    /// <summary>
    /// Сервис для работы с токенами
    /// </summary>
    public interface ITokenService
    {
        Task<TokenPair> GenerateTokens(User user, string? ip = null);
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
    }
}


