using System.Net;
using System.Text.Json;
using Domain.Exceptions;

namespace WebApp.Middleware;

/// <summary>
/// Middleware для автоматической установки HTTP статус кодов на основе исключений
/// </summary>
public class ResultStatusCodeMiddleware
{
    private readonly RequestDelegate _next;

    public ResultStatusCodeMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Выполняем следующий middleware/component
        await _next(context);

        // Если ответ уже отправлен, пропускаем
        if (context.Response.HasStarted)
        {
            return;
        }

        // Проверяем на наличие BrokenRulesException в контексте (от GlobalExceptionHandlingMiddleware)
        var brokenRulesException = context.Items.TryGetValue(nameof(BrokenRulesException), out var exceptionObj)
            ? exceptionObj as BrokenRulesException
            : null;

        if (brokenRulesException != null)
        {
            var statusCode = DetermineStatusCodeFromBrokenRules(brokenRulesException);
            if (statusCode.HasValue)
            {
                context.Response.StatusCode = statusCode.Value;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonSerializer.Serialize(brokenRulesException.BrokenRules));
            }
            return;
        }

        // Если ответ не успешный (не 200), пропускаем дальнейшую обработку
        if (context.Response.StatusCode != (int)HttpStatusCode.OK)
        {
            return;
        }
    }

    private int? DetermineStatusCodeFromBrokenRules(Exception exception)
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

    private bool IsBrokenRulesException(Exception exception, out List<long> brokenRuleCodes)
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

/// <summary>
/// Расширение для удобного использования middleware
/// </summary>
public static class ResultStatusCodeMiddlewareExtensions
{
    public static IApplicationBuilder UseResultStatusCode(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ResultStatusCodeMiddleware>();
    }
}

