using Domain.Entities;
using Domain.Entities.Enums.VkOrd;
using Domain.Models.Requests;
using Domain.Models.Responses;
using Domain.Repositories.Interfaces.ApiCredentials;
using Domain.Repositories.Interfaces.Users;
using Domain.Services.Interfaces;

namespace Domain.Services.Implementations;

public class ApiCredentialService : IApiCredentialService
{
    private readonly IGetApiCredentialsListRepository _listRepo;
    private readonly IGetApiCredentialByIdRepository _getRepo;
    private readonly ISaveApiCredentialRepository _saveRepo;
    private readonly IDeleteApiCredentialRepository _deleteRepo;
    private readonly IGetUserByIdRepository _getUserRepo;
    private readonly IErirStatusSyncService _syncService;

    public ApiCredentialService(
        IGetApiCredentialsListRepository listRepo,
        IGetApiCredentialByIdRepository getRepo,
        ISaveApiCredentialRepository saveRepo,
        IDeleteApiCredentialRepository deleteRepo,
        IGetUserByIdRepository getUserRepo, 
        IErirStatusSyncService syncService)
    {
        _listRepo = listRepo;
        _getRepo = getRepo;
        _saveRepo = saveRepo;
        _deleteRepo = deleteRepo;
        _getUserRepo = getUserRepo;
        _syncService = syncService;
    }

    public async Task<List<ApiCredentialResponse>> GetAll(Guid userId, CancellationToken cancellationToken)
    {
        var user = await _getUserRepo.GetByGuid(userId, cancellationToken);
        var credentials = await _listRepo.GetListAsync(user.Id, cancellationToken);
        return credentials.Select(MapToResponse).ToList();
    }

    public async Task<List<ApiCredentialResponse>> GetAllByEnvironment(Guid userId, VkOrdApiEnvironmentCode apiEnvironment, CancellationToken cancellationToken)
    {
        var user = await _getUserRepo.GetByGuid(userId, cancellationToken);
        var credentials = await _listRepo.GetListAsync(user.Id, cancellationToken, apiEnvironment);
        return credentials.Select(MapToResponse).ToList();
    }

    public async Task<ApiCredentialResponse?> GetById(Guid publicId, Guid userId, CancellationToken cancellationToken)
    {
        var user = await _getUserRepo.GetByGuid(userId, cancellationToken);
        var credential = await _getRepo.GetByPublicId(publicId, cancellationToken);
        if (credential == null || credential.UserId != user.Id)
            return null;
        return MapToResponse(credential);
    }

    public async Task<ApiCredentialResponse> Create(CreateApiCredentialRequest request, long userId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.TokenPlain))
            throw new ArgumentException("TokenPlain is required", nameof(request.TokenPlain));

        var now = DateTimeOffset.UtcNow;
        var environment = Enum.Parse<VkOrdApiEnvironmentCode>(request.Environment, true);

        var credential = new ApiCredential
        {
            Id = 0,
            UserId = userId,
            ApiEnvironment = environment,
            TokenEncrypted = request.TokenPlain, // plain text, will be encrypted in repository
            DisplayName = request.DisplayName,
            CreatedAt = now,
            UpdatedAt = now
        };

        var saved = await _saveRepo.Save(credential, cancellationToken);
        if (saved == null)
            throw new InvalidOperationException("Failed to save credential");

        await _syncService.SyncLogicalAccount(saved.LogicalAccountId, saved, cancellationToken);

        return MapToResponse(saved);
    }

    public async Task<ApiCredentialResponse?> Update(Guid publicId, UpdateApiCredentialRequest request, long userId, CancellationToken cancellationToken)
    {
        var existing = await _getRepo.GetByPublicId(publicId, cancellationToken);
        if (existing == null || existing.UserId != userId)
            return null;

        var now = DateTimeOffset.UtcNow;

        if (!string.IsNullOrWhiteSpace(request.TokenPlain))
        {
            existing.TokenEncrypted = request.TokenPlain; // plain text
        }

        if (!string.IsNullOrWhiteSpace(request.Environment))
        {
            existing.ApiEnvironment = Enum.Parse<VkOrdApiEnvironmentCode>(request.Environment, true);
        }

        existing.DisplayName = request.DisplayName ?? existing.DisplayName;
        existing.UpdatedAt = now;

        var updated = await _saveRepo.Save(existing, cancellationToken);
        return updated != null ? MapToResponse(updated) : null;
    }

    public async Task<bool> Delete(Guid publicId, long userId, CancellationToken cancellationToken)
    {
        var existing = await _getRepo.GetByPublicId(publicId, cancellationToken);
        if (existing == null || existing.UserId != userId)
            return false;

        return await _deleteRepo.Delete(existing.PublicId, cancellationToken);
    }

    private static ApiCredentialResponse MapToResponse(ApiCredential credential)
    {
        return new ApiCredentialResponse
        {
            PublicId = credential.PublicId,
            Environment = credential.ApiEnvironment.ToString(),
            DisplayName = credential.DisplayName,
            CreatedAt = credential.CreatedAt,
            UpdatedAt = credential.UpdatedAt
        };
    }
}
