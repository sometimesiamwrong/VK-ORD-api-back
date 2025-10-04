using System.Collections.ObjectModel;
using Domain.BrokenRules;

namespace Domain.Exceptions;

/// <summary>
/// Исключение, представляющее ошибки валидации и нарушения логики.
/// </summary>
[Serializable]
public class BrokenRulesException : Exception
{
    private readonly ReadOnlyCollection<BrokenRule> _brokenRules;
    private readonly string _brokenRulesSeparator;

    /// <summary>
    /// Список нарушенных правил.
    /// </summary>
    public ReadOnlyCollection<BrokenRule> BrokenRules => _brokenRules;

    /// <inheritdoc />
    public override string Message
    {
        get
        {
            if (_brokenRules.Count == 0)
                return "Broken rules validation exception.";

            return string.Join(_brokenRulesSeparator, _brokenRules.Select(x => x.Message));
        }
    }

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="BrokenRulesException"/>.
    /// </summary>
    /// <param name="brokenRulesCollection">Коллекция нарушенных правил.</param>
    /// <param name="brokenRulesSeparator">Разделитель для сообщений правил.</param>
    /// <param name="innerException">Внутреннее исключение.</param>
    public BrokenRulesException(
        BrokenRulesCollection brokenRulesCollection,
        string brokenRulesSeparator = "\n",
        Exception? innerException = null)
        : base("Broken rules validation exception.", innerException)
    {
        brokenRulesCollection = brokenRulesCollection ?? throw new ArgumentNullException(nameof(brokenRulesCollection));
        if (string.IsNullOrWhiteSpace(brokenRulesSeparator))
            throw new ArgumentException("Broken rules separator cannot be null or empty.", nameof(brokenRulesSeparator));

        _brokenRules = brokenRulesCollection.ToReadOnlyCollection();
        _brokenRulesSeparator = brokenRulesSeparator;
    }

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="BrokenRulesException"/>.
    /// </summary>
    /// <param name="brokenRuleCode">Код нарушенного правила.</param>
    /// <param name="brokenRuleMessage">Сообщение о нарушенном правиле.</param>
    /// <param name="brokenRulesSeparator">Разделитель для сообщений правил.</param>
    /// <param name="innerException">Внутреннее исключение.</param>
    public BrokenRulesException(
        long brokenRuleCode,
        string brokenRuleMessage,
        string domain = "App",
        string brokenRulesSeparator = "\n",
        Exception innerException = null)
        : this(new BrokenRulesCollection(brokenRuleCode, brokenRuleMessage, domain), brokenRulesSeparator, innerException)
    {
    }

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="BrokenRulesException"/>.
    /// </summary>
    /// <param name="brokenRule">Нарушенное правило.</param>
    /// <param name="brokenRulesSeparator">Разделитель для сообщений правил.</param>
    /// <param name="innerException">Внутреннее исключение.</param>
    public BrokenRulesException(
        BrokenRule brokenRule,
        string brokenRulesSeparator = "\n",
        Exception innerException = null)
        : this(new BrokenRulesCollection(brokenRule), brokenRulesSeparator, innerException)
    {
    }
}
