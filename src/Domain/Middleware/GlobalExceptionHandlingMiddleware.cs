using System.Net;
using System.Text.Json;
using Domain.Exceptions;

namespace Domain.Middleware
{
    /// <summary>
    /// Middleware для обработки исключений
    /// </summary>
    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="GlobalExceptionHandlingMiddleware"/>.
        /// </summary>
        /// <param name="next">Следующий делегат.</param>
        /// <param name="logger">Логгер.</param>
        /// <param name="jsonSerializerOptions">Настройки JSON сериализации.</param>
        public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger, JsonSerializerOptions jsonSerializerOptions)
        {
            _next = next;
            _logger = logger;
            _jsonSerializerOptions = jsonSerializerOptions;
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

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json; charset=utf-8";

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
                case BrokenRulesException brokenRulesException:
                    var statusCode = DetermineStatusCodeFromBrokenRules(brokenRulesException);
                    context.Response.StatusCode = statusCode ?? (int)HttpStatusCode.BadRequest;
                    context.Response.ContentType = "application/json; charset=utf-8";
                    var brokenRulesResponse = JsonSerializer.Serialize(brokenRulesException.BrokenRules, _jsonSerializerOptions);
                    await context.Response.WriteAsync(brokenRulesResponse);
                    return; // Важно: выходим из метода, не переходим к общему JSON ответу
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

            var jsonResponse = JsonSerializer.Serialize(response, _jsonSerializerOptions);
            await context.Response.WriteAsync(jsonResponse);
        }

        private static int? DetermineStatusCodeFromBrokenRules(Exception exception)
        {
            // Проверяем, является ли исключение BrokenRulesException
            if (IsBrokenRulesException(exception, out var brokenRulesCodes))
            {
                // Если есть коды ошибок, определяем статус код на их основе
                if (brokenRulesCodes.Any())
                {
                    // Если есть код 401 (Unauthorized), возвращаем 401
                    if (brokenRulesCodes.Contains(401))
                    {
                        return 401;
                    }

                    // Если есть коды >= 10000, это обычно бизнес-логика ошибки - 400
                    if (brokenRulesCodes.Any(code => code >= 10000))
                    {
                        return 400;
                    }

                    // Для других кодов ошибок возвращаем 400
                    return 400;
                }
            }

            // Для остальных исключений возвращаем null (не меняем статус код)
            return null;
        }

        private static bool IsBrokenRulesException(Exception exception, out List<long> brokenRuleCodes)
        {
            brokenRuleCodes = new List<long>();

            // Проверяем, является ли исключение нашим BrokenRulesException
            if (exception is BrokenRulesException brokenRulesException)
            {
                // Извлекаем коды из нашей коллекции BrokenRules
                foreach (var brokenRule in brokenRulesException.BrokenRules)
                {
                    brokenRuleCodes.Add(brokenRule.Code);
                }
                return true;
            }

            return false;
        }
    }
}
