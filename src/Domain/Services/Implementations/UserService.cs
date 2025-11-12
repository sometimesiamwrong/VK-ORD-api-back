using Domain.Entities;
using Domain.Models.Requests;
using Domain.Models.Responses;
using Domain.Repositories.Interfaces.Users;
using Domain.Services.Interfaces;

namespace Domain.Services.Implementations;

public class UserService : IUserService
{
    private readonly IGetUserByIdRepository _getRepo;
    private readonly ISaveUserRepository _saveRepo;

    public UserService(IGetUserByIdRepository getRepo, ISaveUserRepository saveRepo)
    {
        _getRepo = getRepo;
        _saveRepo = saveRepo;
    }

    public async Task<UserProfileResponse?> Get(long userId, CancellationToken cancellationToken)
    {
        var user = await _getRepo.GetById(userId, cancellationToken);
        if (user == null)
            return null;
        return MapToResponse(user);
    }

    public async Task<UserProfileResponse?> Update(long userId, UpdateUserRequest request, CancellationToken cancellationToken)
    {
        var existing = await _getRepo.GetById(userId, cancellationToken);
        if (existing == null)
            return null;

        var now = DateTimeOffset.UtcNow;

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            existing.Name = request.Name;
        }

        existing.UpdatedAt = now;

        var updated = await _saveRepo.Save(existing, cancellationToken);
        return updated != null ? MapToResponse(updated) : null;
    }

    private static UserProfileResponse MapToResponse(User user)
    {
        return new UserProfileResponse
        {
            PublicId = user.PublicId,
            UserName = user.UserName,
            Name = user.Name,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }
}
