using Microsoft.AspNetCore.Mvc;

namespace VkOrdApiWrapper.Models.Requests
{
    /// <summary>
    /// Базовая модель запроса для авторизованных эндпоинтов
    /// UserId и окружение могут быть переданы через заголовки,
    /// но приоритет всегда за значением из JWT claims.
    /// </summary>
    public abstract class AuthorizedRequestBase
    {
        /// <summary>
        /// Идентификатор пользователя. Проброс через заголовок "x-user-id" (опционально).
        /// Используется только для сверки с клеймом JWT.
        /// </summary>
        [FromHeader(Name = "x-user-id")]
        public Guid? UserId { get; set; }

        /// <summary>
        /// Окружение VK ORD. Проброс через заголовок "x-api-vk-env" (sandbox|prod). Опционально.
        /// </summary>
        [FromHeader(Name = "x-api-vk-env")]
        public string? Environment { get; set; }
    }
}


