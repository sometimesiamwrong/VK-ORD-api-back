namespace Domain.BrokenRules;

/// <summary>
/// Представляет нарушенное правило бизнес-логики.
/// </summary>
public class BrokenRule
{
    /// <summary>
    /// Код правила.
    /// </summary>
    public long Code { get; }

    /// <summary>
    /// Домен правила.
    /// </summary>
    public string Domain { get; }

    /// <summary>
    /// Сообщение об ошибке.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="BrokenRule"/>.
    /// </summary>
    /// <param name="code">Код правила.</param>
    /// <param name="message">Сообщение об ошибке.</param>
    /// <param name="domain">Домен правила.</param>
    public BrokenRule(long code, string message, string domain = "App")
    {
        Message = message ?? throw new ArgumentNullException(nameof(message));
        Code = code;
        Domain = domain;
    }

    /// <summary>
    /// Возвращает строковое представление объекта.
    /// </summary>
    public override string ToString()
    {
        return $"[{Code}]-[{Domain}] {Message}";
    }
}
