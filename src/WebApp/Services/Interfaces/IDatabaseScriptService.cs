namespace WebApp.Services.Interfaces;

/// <summary>
/// Сервис для управления выполнением SQL-скриптов при запуске приложения
/// </summary>
public interface IDatabaseScriptService
{
    /// <summary>
    /// Выполняет все новые SQL-скрипты из указанной директории
    /// </summary>
    /// <param name="scriptsPath">Путь к директории со скриптами</param>
    /// <returns>Количество выполненных скриптов</returns>
    Task<int> ExecutePendingScriptsAsync(string scriptsPath = "Scripts/");

    /// <summary>
    /// Проверяет, был ли выполнен скрипт с указанным именем
    /// </summary>
    /// <param name="scriptName">Имя скрипта</param>
    /// <returns>True, если скрипт уже выполнен</returns>
    Task<bool> IsScriptExecutedAsync(string scriptName);

    /// <summary>
    /// Выполняет конкретный SQL-скрипт
    /// </summary>
    /// <param name="scriptName">Имя скрипта</param>
    /// <param name="scriptContent">Содержимое скрипта</param>
    /// <param name="description">Описание скрипта</param>
    /// <returns>True, если выполнение прошло успешно</returns>
    Task<bool> ExecuteScriptAsync(string scriptName, string scriptContent, string? description = null);
}
