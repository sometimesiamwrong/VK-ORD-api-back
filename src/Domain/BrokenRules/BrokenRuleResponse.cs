namespace Domain.BrokenRules;

/// <summary>
/// Ответ на нарушенное правило бизнес-логики.
/// </summary>
public class BrokenRuleResponse
{
    /// <summary>
    /// Сообщение о нарушенном правиле.
    /// </summary>
    public required string Message { get; set; }

    /// <summary>
    /// Код нарушенного правила.
    /// </summary>
    public long Code { get; set; }

    /// <summary>
    /// Домен нарушенного правила.
    /// </summary>
    public required string Domain { get; set; }
}
