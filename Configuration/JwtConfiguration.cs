namespace VkOrdApiWrapper.Configuration
{
    /// <summary>
    /// Конфигурация JWT токенов
    /// </summary>
    public class JwtConfiguration
    {
        /// <summary>
        /// Секретный ключ для генерации JWT токенов
        /// </summary>
        public string SecretKey { get; set; }

        /// <summary>
        /// Издатель JWT токена
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// Аудитория JWT токена
        /// </summary>
        public string Audience { get; set; }

        /// <summary>
        /// Время жизни JWT токена в минутах
        /// </summary>
        public int ExpiryInMinutes { get; set; }

        /// <summary>
        /// Время жизни refresh токена в днях
        /// </summary>
        public int RefreshTokenDays { get; set; } = 30;

        /// <summary>
        /// Имя cookie для refresh токена
        /// </summary>
        public string RefreshCookieName { get; set; } = "refresh_token";
    }
}
