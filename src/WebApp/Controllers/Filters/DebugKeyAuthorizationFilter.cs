using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using WebApp.Configuration;

namespace WebApp.Controllers.Filters;

public class DebugKeyAuthorizationFilter : IActionFilter
{
    private readonly DebugSettings _settings;

    public DebugKeyAuthorizationFilter(IOptions<DebugSettings> settings)
    {
        _settings = settings.Value;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        var request = context.HttpContext.Request;
        var debugKey = request.Headers["X-Debug-Key"].FirstOrDefault();

        if (string.IsNullOrWhiteSpace(debugKey) || debugKey != _settings.SecretKey)
        {
            context.Result = new UnauthorizedObjectResult(new
            {
                error = "Unauthorized",
                message = "Invalid or missing X-Debug-Key header"
            });
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
}
