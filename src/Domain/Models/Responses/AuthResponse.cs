namespace Domain.Models.Responses
{
    /// <summary>
    /// Ответ при успешной аутентификации
    /// </summary>
    public class AuthResponse
    {
        /// <summary>
        /// Токен
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Тип токена
        /// </summary>
        public string TokenType { get; set; } = "Bearer";

        /// <summary>
        /// Время жизни токена в секундах
        /// </summary>
        public int ExpiresIn { get; set; } // в секундах

        /// <summary>
        /// Время выпуска токена
        /// </summary>
        public DateTime IssuedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Время истечения токена
        /// </summary>
        public DateTime ExpiresAt { get; set; }

        /// <summary>
        /// Refresh токен
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// Создать ответ с токеном
        /// </summary>
        public static AuthResponse Create(string token, int expiresInSeconds, string refreshToken)
        {
            return new AuthResponse
            {
                Token = token,
                ExpiresIn = expiresInSeconds,
                ExpiresAt = DateTime.UtcNow.AddSeconds(expiresInSeconds),
                RefreshToken = refreshToken
            };
        }
    }

    /// <summary>
    /// Ответ при ошибке аутентификации
    /// </summary>
    public class AuthErrorResponse
    {
        /// <summary>
        /// Код ошибки
        /// </summary>
        public string ErrorCode { get; set; }

        /// <summary>
        /// Создать ответ при ошибке аутентификации
        /// </summary>
        public static AuthErrorResponse InvalidCredentials() => new()
        {
            ErrorCode = "INVALID_CREDENTIALS"
        };

        /// <summary>
        /// Создать ответ при заблокированном аккаунте
        /// </summary>
        public static AuthErrorResponse AccountLocked() => new()
        {
            ErrorCode = "ACCOUNT_LOCKED"
        };
    }
}
