using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using VkOrdApiWrapper.Configuration;
using VkOrdApiWrapper.Data;
using VkOrdApiWrapper.Entities;
using VkOrdApiWrapper.Models.Responses;
using VkOrdApiWrapper.Security;

namespace VkOrdApiWrapper.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : BaseApiController
    {
        private readonly JwtConfiguration _jwtConfig;
        private readonly ApplicationDbContext _db;
        private readonly ITokenService _tokenService;
        private readonly PasswordHasher<User> _hasher = new();

        public AuthController(IOptions<JwtConfiguration> jwtConfig, ApplicationDbContext db, ITokenService tokenService)
        {
            _jwtConfig = jwtConfig.Value;
            _db = db;
            _tokenService = tokenService;
        }

        /// <summary>
        /// Получить JWT токен для доступа к API
        /// </summary>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ApiResponse<AuthResponse>> Register([FromBody] RegisterRequest request)
        {
            var exists = await _db.Users.AnyAsync(u => u.UserName == request.UserName);
            if (exists)
            {
                HttpContext.Response.StatusCode = 400;
                return Error<AuthResponse>("Username is already taken");
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = request.UserName,
                Name = request.Name,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            user.PasswordHash = _hasher.HashPassword(user, request.Password);
            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            var tokens = _tokenService.GenerateTokens(user, null, HttpContext.Connection.RemoteIpAddress?.ToString());
            SetRefreshCookie(tokens.RefreshToken);
            var authResponse = AuthResponse.Create(tokens.AccessToken, _jwtConfig.ExpiryInMinutes * 60, tokens.RefreshToken);
            return Ok(authResponse, "Registered and authenticated");
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ApiResponse<AuthResponse>> Login([FromBody] LoginRequest request)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.UserName == request.UserName);
            if (user == null || _hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password) == PasswordVerificationResult.Failed)
            {
                HttpContext.Response.StatusCode = 401;
                return Error<AuthResponse>("Invalid credentials");
            }

            var tokens = _tokenService.GenerateTokens(user, request.DeviceId, HttpContext.Connection.RemoteIpAddress?.ToString());
            SetRefreshCookie(tokens.RefreshToken);
            var authResponse = AuthResponse.Create(tokens.AccessToken, _jwtConfig.ExpiryInMinutes * 60, tokens.RefreshToken);
            return Ok(authResponse, "Authentication successful");
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<ApiResponse<AuthResponse>> Refresh()
        {
            var refresh = Request.Cookies[_jwtConfig.RefreshCookieName];
            if (string.IsNullOrEmpty(refresh))
            {
                HttpContext.Response.StatusCode = 401;
                return Error<AuthResponse>("Missing refresh token");
            }

            var refreshHash = ComputeSha256(refresh);
            var entity = await _db.RefreshTokens.Include(r => r.User)
                .FirstOrDefaultAsync(r => r.TokenHash == refreshHash && r.RevokedAt == null && r.ExpiresAt > DateTime.UtcNow);
            if (entity == null)
            {
                HttpContext.Response.StatusCode = 401;
                return Error<AuthResponse>("Invalid refresh token");
            }

            // rotate
            entity.RevokedAt = DateTime.UtcNow;
            var newTokens = _tokenService.GenerateTokens(entity.User, entity.DeviceId, HttpContext.Connection.RemoteIpAddress?.ToString());
            entity.ReplacedByTokenHash = ComputeSha256(newTokens.RefreshToken);
            await _db.SaveChangesAsync();

            SetRefreshCookie(newTokens.RefreshToken);
            var authResponse = AuthResponse.Create(newTokens.AccessToken, _jwtConfig.ExpiryInMinutes * 60, newTokens.RefreshToken);
            return Ok(authResponse, "Token refreshed");
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<ApiResponse> Logout()
        {
            var refresh = Request.Cookies[_jwtConfig.RefreshCookieName];
            if (!string.IsNullOrEmpty(refresh))
            {
                var hash = ComputeSha256(refresh);
                var token = await _db.RefreshTokens.FirstOrDefaultAsync(r => r.TokenHash == hash && r.RevokedAt == null);
                if (token != null)
                {
                    token.RevokedAt = DateTime.UtcNow;
                    await _db.SaveChangesAsync();
                }
            }

            Response.Cookies.Delete(_jwtConfig.RefreshCookieName, new CookieOptions
            {
                Path = "/",
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });

            return Ok("Logged out");
        }

        private static string ComputeSha256(string input)
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            var hash = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));
            return Convert.ToBase64String(hash);
        }

        private void SetRefreshCookie(string refreshToken)
        {
            Response.Cookies.Append(_jwtConfig.RefreshCookieName, refreshToken, new CookieOptions
            {
                Path = "/",
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(_jwtConfig.RefreshTokenDays)
            });
        }
    }

    /// <summary>
    /// Запрос на получение токена
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Пароль
        /// </summary>
        public string Password { get; set; }

        public string? DeviceId { get; set; }
    }

    public class RegisterRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string? Name { get; set; }
    }
}
