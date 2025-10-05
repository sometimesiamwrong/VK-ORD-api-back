using Domain.Entities;
using Domain.Entities.Enums;

namespace WebApp.Repositories.Interfaces.ApiCredentials
{
    /// <summary>
    /// Репозиторий для получения списка ApiCredentials
    /// </summary>
    public interface IGetApiCredentialsListRepository
    {
        /// <summary>
        /// Получить список ApiCredentials для пользователя
        /// </summary>
        Task<List<ApiCredential>> GetListAsync(long userId, CancellationToken cancellationToken, VkOrdEnvironmentCode? environment = null);
    }
}
