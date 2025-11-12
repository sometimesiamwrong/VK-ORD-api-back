using Domain.Models.Requests;
using Domain.Models.Responses;

namespace Domain.Services.Interfaces;

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
