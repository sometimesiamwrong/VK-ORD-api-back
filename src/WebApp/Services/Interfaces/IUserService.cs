using System.Threading;
using System.Threading.Tasks;
using WebApp.Models.Requests;
using WebApp.Models.Responses;
using WebApp.Services.Interfaces;

namespace WebApp.Services.Interfaces;

public interface IUserService
{
    /// <summary>
    /// Получить текущего пользователя
    /// </summary>
    /// <returns>Профиль пользователя</returns>
    Task<UserProfileResponse?> Get(long userId, CancellationToken cancellationToken);

    /// <summary>
    /// Обновить текущего пользователя
    /// </summary>
    /// <returns>Профиль пользователя</returns>
    Task<UserProfileResponse?> Update(long userId, UpdateUserRequest request, CancellationToken cancellationToken);
}
