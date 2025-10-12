using System;
using Domain.Entities.Enums;

namespace Domain.Entities
{
    /// <summary>
    /// Контекст для работы с VK API
    /// </summary>
    public class VkOrdApiContext
    {
        /// <summary>
        /// API ключ для аутентификации
        /// </summary>
        public string ApiKey { get; set; } = string.Empty;

        /// <summary>
        /// Маршрут API (production/test)
        /// </summary>
        public VkOrdApiEnvironmentCode Route { get; set; }

        /// <summary>
        /// Проверка валидности контекста
        /// </summary>
        /// <returns>True если контекст валиден</returns>
        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(ApiKey) && Route != VkOrdApiEnvironmentCode.Unknown;
        }

        /// <summary>
        /// Получить базовый URL для API
        /// </summary>
        /// <returns>Базовый URL в зависимости от маршрута</returns>
        public string GetBaseUrl()
        {
            return Route switch
            {
                VkOrdApiEnvironmentCode.Production => "https://api.ord.vk.com",
                VkOrdApiEnvironmentCode.Sandbox => "https://api-sandbox.ord.vk.com",
                _ => throw new ArgumentException("Invalid Route", nameof(Route))
            };
        }

        /// <summary>
        /// Получить заголовок авторизации
        /// </summary>
        /// <returns>Строка для заголовка Authorization</returns>
        public string GetAuthorizationHeader()
        {
            return $"Bearer {ApiKey}";
        }
    }
}
