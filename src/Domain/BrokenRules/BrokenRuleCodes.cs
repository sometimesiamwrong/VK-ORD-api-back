using System.ComponentModel;
using Domain.Attributes;

namespace Domain.BrokenRules;

/// <summary>
/// Коды для Broken rule exception.
/// </summary>
public enum BrokenRuleCodes : long
{
    /// <summary>
    /// Неизвестная ошибка
    /// </summary>
    [Description("Неизвестная ошибка")]
    [Domain("App")]
    Unknown = 0,

    /// <summary>
    /// Не найдены данные по VK ОР API ключу
    /// </summary>
    [Description("Не найдены данные по VK ОРД API ключу")]
    [Domain("Database")]
    VkOrdCredentialsNotFound = 1,
    
    /// <summary>
    /// Не найдены данные по VK ОР API ключу
    /// </summary>
    [Description("Не найден заголовок ключа VK ОРД API")]
    [Domain("Request")]
    VkOrdCredentialsHeaderNotFound = 2,

    /// <summary>
    /// Пользователь уже существует
    /// </summary>
    [Description("Пользователь с таким именем уже существует")]
    [Domain("User")]
    UserWithSuchNameAlreadyExists = 3,

    /// <summary>
    /// Неверные учетные данные
    /// </summary>
    [Description("Неверные учетные данные")]
    [Domain("User")]
    InvalidCredentials = 4,

    /// <summary>
    /// Пользователь не авторизован
    /// </summary>
    [Description("Пользователь не авторизован")]
    [Domain("User")]
    UserNotAuthorized = 5,
}