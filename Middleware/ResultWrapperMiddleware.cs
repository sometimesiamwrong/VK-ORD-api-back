using Microsoft.AspNetCore.Mvc;
using VkOrdApiWrapper.Models.Responses;

namespace VkOrdApiWrapper.Middleware
{
    /// <summary>
    /// Middleware для автоматической обертки результатов контроллеров в API ответы
    /// </summary>
    public class ResultWrapperMiddleware
    {
        private readonly RequestDelegate _next;

        public ResultWrapperMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Выполняем следующий middleware/component
            await _next(context);

            // Если результат уже установлен (например, через IActionResult), пропускаем
            if (context.Response.HasStarted || context.Response.StatusCode != 200)
            {
                return;
            }

            // Проверяем, есть ли результат в HttpContext.Items
            if (context.Items.TryGetValue("ApiResponse", out var response))
            {
                // Обертываем результат в JSON ответ
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }

    /// <summary>
    /// Расширение для удобного использования middleware
    /// </summary>
    public static class ResultWrapperMiddlewareExtensions
    {
        public static IApplicationBuilder UseResultWrapper(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ResultWrapperMiddleware>();
        }
    }
}
