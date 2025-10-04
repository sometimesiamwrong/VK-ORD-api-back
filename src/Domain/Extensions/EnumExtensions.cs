using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using Domain.Attributes;

namespace Domain.Extensions;

/// <summary>
/// Расширения для работы с Enum
/// </summary>
[ExcludeFromCodeCoverage]
public static class EnumExtensions
{
    public static string GetDescription(this Enum enumValue)
    {
        var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());
        var descriptionAttribute = fieldInfo?.GetCustomAttribute<DescriptionAttribute>();
        return descriptionAttribute?.Description ?? enumValue.ToString();
    }

    public static string GetDomain(this Enum enumValue)
    {
        var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());
        var domainAttribute = fieldInfo?.GetCustomAttribute<DomainAttribute>();
        return domainAttribute?.Domain ?? "App";
    }
}