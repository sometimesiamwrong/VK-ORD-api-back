using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WebApp.Configuration;
using WebApp.Repositories.Interfaces.RefreshTokens;

namespace WebApp.Security
{
    public class TokenService : ITokenService
    {
        private readonly JwtConfiguration _jwt;
        private readonly ISaveRefreshTokenRepository _saveRefreshTokenRepo;

        public TokenService(IOptions<JwtConfiguration> jwtOptions, ISaveRefreshTokenRepository saveRefreshTokenRepo)
        {
            _jwt = jwtOptions.Value;
            _saveRefreshTokenRepo = saveRefreshTokenRepo;
        }

        public async Task<TokenPair> GenerateTokens(User user, string? ip = null)
        {
            var accessToken = GenerateAccessToken(user);
            var refreshToken = GenerateRefreshToken();

            var refreshHash = ComputeSha256(refreshToken);
            var refreshEntity = new RefreshToken
            {
                UserId = user.Id,
                TokenHash = refreshHash,
                ExpiresAt = DateTime.UtcNow.AddDays(_jwt.RefreshTokenDays),
                CreatedAt = DateTime.UtcNow,
                CreatedByIp = ip,
                DeviceId = null
            };
            await _saveRefreshTokenRepo.SaveAsync(refreshEntity);

            return new TokenPair(
                accessToken,
                refreshToken,
                DateTime.UtcNow.AddMinutes(_jwt.ExpiryInMinutes),
                refreshEntity.ExpiresAt);
        }

        public string GenerateAccessToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };
            var token = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwt.ExpiryInMinutes),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var bytes = RandomNumberGenerator.GetBytes(64);
            return Convert.ToBase64String(bytes);
        }

        public static string ComputeSha256(string input)
        {
            using var sha = SHA256.Create();
            var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToBase64String(hash);
        }
    }
}


