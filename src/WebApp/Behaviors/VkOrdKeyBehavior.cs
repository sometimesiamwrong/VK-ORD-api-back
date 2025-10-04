using MediatR;
using Domain.BrokenRules;
using Domain.Extensions;
using Microsoft.AspNetCore.Http;
using WebApp.Models.Requests;

namespace WebApp.Behaviors;

public class VkOrdKeyBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public VkOrdKeyBehavior(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request is IRequestWithVkOrdKey)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                throw new InvalidOperationException("HttpContext is not available.");
            }

            var idStr = httpContext.Request.Headers["x-vkord-credential-id"].FirstOrDefault();
            if (string.IsNullOrWhiteSpace(idStr))
            {
                throw BrokenRuleCodes.VkOrdCredentialsHeaderNotFound.AsExn();
            }

            if (!Guid.TryParse(idStr, out var credentialId))
            {
                throw new FormatException($"Invalid GUID format for x-vkord-credential-id: {idStr}");
            }

            var prop = typeof(TRequest).GetProperty(nameof(IRequestWithVkOrdKey.ApiCredentialPublicId));
            prop?.SetValue(request, credentialId);
        }

        return await next();
    }
}
