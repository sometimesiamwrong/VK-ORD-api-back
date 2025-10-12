using System.Collections.Generic;
using Domain.Entities.Enums;
using WebApp.Models.Requests;
using WebApp.Models.Responses;
using WebApp.Services.Interfaces;

namespace WebApp.Services.Interfaces;

public interface IApiCredentialService
{
    Task<List<ApiCredentialResponse>> GetAll(Guid userId, CancellationToken cancellationToken);
    Task<List<ApiCredentialResponse>> GetAllByEnvironment(Guid userId, VkOrdApiEnvironmentCode apiEnvironment, CancellationToken cancellationToken);
    Task<ApiCredentialResponse?> GetById(Guid publicId, Guid userId, CancellationToken cancellationToken);
    Task<ApiCredentialResponse> Create(CreateApiCredentialRequest request, long userId, CancellationToken cancellationToken);
    Task<ApiCredentialResponse?> Update(Guid publicId, UpdateApiCredentialRequest request, long userId, CancellationToken cancellationToken);
    Task<bool> Delete(Guid publicId, long userId, CancellationToken cancellationToken);
}
