using System.Net;
using Domain.Exceptions;
using Newtonsoft.Json;

namespace WebApp.Middleware
{
    /// <summary>
    /// Middleware для обработки исключений
    /// </summary>
    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="GlobalExceptionHandlingMiddleware"/>.
        /// </summary>
        /// <param name="next">Следующий делегат.</param>
        /// <param name="logger">Логгер.</param>
        public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = new
            {
                Error = "Internal Server Error",
                ErrorCode = 100,
                Message = exception.Message,
                Timestamp = DateTimeOffset.UtcNow
            };

            // Сохраняем исключение в контексте для обработки в ResultStatusCodeMiddleware
            context.Items["Exception"] = exception;

            switch (exception)
            {
                case var ex when ex.GetType().Name.Contains(nameof(BrokenRulesException)):
                    // Сохраняем BrokenRulesException для обработки в ResultStatusCodeMiddleware
                    context.Items[nameof(BrokenRulesException)] = ex;
                    // Не устанавливаем статус код здесь, оставляем для middleware
                    context.Response.StatusCode = (int)HttpStatusCode.Continue; // Временно OK, middleware изменит
                    response = new { Error = "Broken Rules", ErrorCode = 100, Message = exception.Message, Timestamp = DateTimeOffset.UtcNow };
                    break;
                case ArgumentException:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response = new { Error = "Bad Request", ErrorCode = 400, Message = exception.Message, Timestamp = DateTimeOffset.UtcNow };
                    break;
                case UnauthorizedAccessException:
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    response = new { Error = "Unauthorized", ErrorCode = 401, Message = exception.Message, Timestamp = DateTimeOffset.UtcNow };
                    break;
                case HttpRequestException:
                    context.Response.StatusCode = (int)HttpStatusCode.BadGateway;
                    response = new { Error = "External Service Error", ErrorCode = 502, Message = exception.Message, Timestamp = DateTimeOffset.UtcNow };
                    break;
                default:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    response = new { Error = "Internal Server Error", ErrorCode = 500, Message = exception.Message, Timestamp = DateTimeOffset.UtcNow };
                    break;
            }

            var jsonResponse = JsonConvert.SerializeObject(response);
            await context.Response.WriteAsync(jsonResponse);
        }
    }
}
