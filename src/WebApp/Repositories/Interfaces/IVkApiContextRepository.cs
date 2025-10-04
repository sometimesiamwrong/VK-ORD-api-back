using Domain.Entities;

namespace WebApp.Repositories.Interfaces
{
    /// <summary>
    /// Репозиторий для получения контекста VK API
    /// </summary>
    public interface IVkApiContextRepository
    {
        /// <summary>
        /// Получить контекст VK API для пользователя
        /// </summary>
        Task<VkApiContext> GetVkApiContextAsync(long id, long userId);
    }
}
