using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using VkOrdApiWrapper.Configuration;
using VkOrdApiWrapper.Models.Responses;

namespace VkOrdApiWrapper.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : BaseApiController
    {
        private readonly JwtConfiguration _jwtConfig;

        public AuthController(IOptions<JwtConfiguration> jwtConfig)
        {
            _jwtConfig = jwtConfig.Value;
        }

        /// <summary>
        /// Получить JWT токен для доступа к API
        /// </summary>
        [HttpPost("token")]
        [AllowAnonymous]
        public ApiResponse<AuthResponse> GetToken([FromBody] TokenRequest request)
        {
            // Здесь должна быть ваша логика проверки учетных данных
            // Для примера используем простую проверку
            if (request.Username == "admin" && request.Password == "password")
            {
                var token = GenerateJwtToken(request.Username);
                var authResponse = AuthResponse.Create(token, _jwtConfig.ExpiryInMinutes * 60);
                return Ok(authResponse, "Authentication successful");
            }

            // Для ошибок аутентификации устанавливаем статус код вручную
            HttpContext.Response.StatusCode = 401;
            return Error<AuthResponse>("Invalid credentials");
        }

        /// <summary>
        /// Генерирует JWT токен
        /// </summary>
        private string GenerateJwtToken(string username)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtConfig.SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(_jwtConfig.ExpiryInMinutes),
                Issuer = _jwtConfig.Issuer,
                Audience = _jwtConfig.Audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }

    /// <summary>
    /// Запрос на получение токена
    /// </summary>
    public class TokenRequest
    {
        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Пароль
        /// </summary>
        public string Password { get; set; }
    }
}
