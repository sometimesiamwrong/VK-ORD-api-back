using Domain.BrokenRules;
using Domain.Data;
using Domain.Exceptions;
using Domain.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Controllers;

public abstract class BaseController : ControllerBase
{
    protected long UserId => HttpContext.User.GetUserId();

    protected Guid ApiCredentialPublicId() 
    {
        var id = HttpContext.Request.Headers["x-vkord-credential-id"].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(id)) {
            throw BrokenRuleCodes.VkOrdCredentialsHeaderNotFound.AsExn();
        }
        return Guid.Parse(id);
    }

    /// <summary>
    /// Получить ID ApiCredential из заголовка запроса
    /// </summary>
    protected async Task<long> GetApiCredentialIdAsync(AppDbContext context, CancellationToken cancellationToken = default)
    {
        var publicId = ApiCredentialPublicId();
        
        var credential = await context.ApiCredentials
            .Where(a => a.PublicId == publicId && !a.IsDeleted)
            .Select(a => new { a.Id })
            .FirstOrDefaultAsync(cancellationToken);

        if (credential == null)
        {
            throw new BrokenRulesException(404, "API Credential not found", "ApiCredential");
        }

        return credential.Id;
    }
}
