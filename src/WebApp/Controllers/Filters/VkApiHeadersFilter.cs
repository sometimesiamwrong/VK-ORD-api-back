using Domain.BrokenRules;
using Domain.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApp.Controllers.Filters;

/// <summary>
/// Фильтр для проверки обязательных заголовков VK API
/// </summary>
public class VkApiHeadersFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        var request = context.HttpContext.Request;

        var apiKeyId = request.Headers["x-vkord-credential-id"].FirstOrDefault();

        if (string.IsNullOrWhiteSpace(apiKeyId))
        {
            context.Result = new BadRequestObjectResult(BrokenRuleCodes.VkOrdCredentialsHeaderNotFound.AsBrokenRule());
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // Не требуется
    }
}
