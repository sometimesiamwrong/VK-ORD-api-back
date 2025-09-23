using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace VkOrdApiWrapper.Controllers.Filters;

/// <summary>
/// Фильтр для проверки обязательных заголовков VK API
/// </summary>
public class VkApiHeadersFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        var request = context.HttpContext.Request;

        var apiKey = request.Headers["x-api-vk-key"].FirstOrDefault();
        var route = request.Headers["x-api-vk-route"].FirstOrDefault();

        if (string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(route))
        {
            context.Result = new BadRequestObjectResult(new
            {
                Status = "error",
                Message = "Missing required headers: x-api-vk-key and x-api-vk-route",
                Code = "MISSING_HEADERS"
            });
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // Не требуется
    }
}
