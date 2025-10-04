using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Domain.Attributes;
using Domain.BrokenRules;
using Domain.Exceptions;

namespace Domain.Extensions;

/// <summary>
/// Расширения для работы с BrokenRules
/// </summary>
[ExcludeFromCodeCoverage]
public static class BrokenRulesExtensions
{
    /// <summary>
    /// Получить <see cref="BrokenRulesException"/> из <see cref="BrokenRuleCodes"/> с заданным сообщением message
    /// </summary>
    /// <param name="brokenRuleCode"><inheritdoc cref="BrokenRuleCodes"/></param>
    /// <param name="message">
    /// Сообщение, если не задано,
    /// то сначала идет попытка получить значение <see cref="DescriptionAttribute"/>,
    /// если таковое не нашлось, то сообщением будет <see cref="Enum.ToString()"/>
    /// </param>
    public static BrokenRulesException AsExn(this BrokenRuleCodes brokenRuleCode, string? message = null, string? domain = null)
    {
        var msg = message
                  ?? brokenRuleCode.GetDescription()
                  ?? brokenRuleCode.ToString();
        
        domain = domain ?? brokenRuleCode.GetDomain() ?? "App";

        return new BrokenRulesException((long)brokenRuleCode, msg, domain);
    }

    /// <summary>
    /// Получить <see cref="BrokenRule"/> из <see cref="BrokenRuleCodes"/> с заданным сообщением message
    /// </summary>
    /// <param name="brokenRuleCode"><inheritdoc cref="BrokenRuleCodes"/></param>
    /// <param name="message">
    /// Сообщение, если не задано,
    /// то сначала идет попытка получить значение <see cref="DescriptionAttribute"/>,
    /// если таковое не нашлось, то сообщением будет <see cref="Enum.ToString()"/>
    /// </param>
    public static BrokenRule AsBrokenRule(this BrokenRuleCodes brokenRuleCode, string? message = null, string? domain = null)
    {
        var msg = message
                  ?? brokenRuleCode.GetDescription()
                  ?? brokenRuleCode.ToString();

        domain = domain ?? brokenRuleCode.GetDomain() ?? "App";

        return new BrokenRule((long)brokenRuleCode, msg, domain);
    }

    /// <summary>
    /// Выбросить исключение <see cref="BrokenRulesException"/> из <see cref="BrokenRuleCodes"/> с заданным сообщением message
    /// </summary>
    /// <param name="brokenRuleCode"><inheritdoc cref="BrokenRuleCodes"/></param>
    /// <param name="message">
    /// Сообщение, если не задано,
    /// то сначала идет попытка получить значение <see cref="DescriptionAttribute"/>,
    /// если таковое не нашлось, то сообщением будет <see cref="Enum.ToString()"/>
    /// </param>
    public static void Throw(this BrokenRuleCodes brokenRuleCode, string? message = null, string? domain = null)
    {
        throw brokenRuleCode.AsExn(message, domain);
    }

    /// <summary>
    /// Получить <see cref="DescriptionAttribute"/> из <see cref="BrokenRuleCodes"/>
    /// </summary>
    /// <returns><see cref="DescriptionAttribute.Description"/></returns>
    public static string GetDescription<T>(this T @enum) where T : Enum
    {
        var result =
            typeof(T)
                .GetFields()
                .FirstOrDefault(p => p.Name == @enum.ToString())
                ?.GetCustomAttributes(false)
                .OfType<DescriptionAttribute>()
                .FirstOrDefault()
                ?.Description ?? string.Empty;

        return result;
    }

    /// <summary>
    /// Получить <see cref="DomainAttribute"/> из <see cref="BrokenRuleCodes"/>
    /// </summary>
    /// <returns><see cref="DomainAttribute.Domain"/></returns>
    public static string GetDomain<T>(this T @enum) where T : Enum
    {
        var result =
            typeof(T)
                .GetFields()
                .FirstOrDefault(p => p.Name == @enum.ToString())
                ?.GetCustomAttributes(false)
                .OfType<DomainAttribute>()
                .FirstOrDefault()
                ?.Domain ?? string.Empty;

        return result;
    }
}
