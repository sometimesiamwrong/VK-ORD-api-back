using Domain.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebApp.Models.Requests;

namespace WebApp.Filters
{
    /// <summary>
    /// Глобальный фильтр: переносит userId из JWT в модель AuthorizedRequestBase и валидирует x-user-id.
    /// </summary>
    public class EnsureAuthorizedRequestFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var user = context.HttpContext.User;
            if (user?.Identity?.IsAuthenticated != true) return;

            Guid userIdFromJwt;
            try { userIdFromJwt = user.GetUserId(); }
            catch { return; }

            foreach (var kv in context.ActionArguments.ToArray())
            {
                if (kv.Value is AuthorizedRequestBase authBase)
                {
                    if (authBase.UserId.HasValue && authBase.UserId.Value != userIdFromJwt)
                    {
                        context.Result = new BadRequestObjectResult("x-user-id does not match JWT subject");
                        return;
                    }
                    authBase.UserId = userIdFromJwt;
                }
            }
        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}


