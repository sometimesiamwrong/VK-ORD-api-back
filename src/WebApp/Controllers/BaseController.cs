using Domain.BrokenRules;
using Domain.Extensions;
using Microsoft.AspNetCore.Mvc;

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
}
