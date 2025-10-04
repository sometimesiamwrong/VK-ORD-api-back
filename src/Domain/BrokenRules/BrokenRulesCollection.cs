using System.Collections;
using System.Collections.ObjectModel;

namespace Domain.BrokenRules;

/// <summary>
/// Коллекция нарушенных правил бизнес-логики.
/// </summary>
public class BrokenRulesCollection : IEnumerable<BrokenRule>
{
    private readonly List<BrokenRule> _brokenRules = new();

    /// <summary>
    /// Количество элементов в коллекции.
    /// </summary>
    public int Count => _brokenRules.Count;

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="BrokenRulesCollection"/>.
    /// </summary>
    public BrokenRulesCollection()
    {
    }

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="BrokenRulesCollection"/> с одним правилом.
    /// </summary>
    /// <param name="message">Сообщение об ошибке.</param>
    /// <param name="code">Код правила.</param>
    /// <param name="domain">Домен правила.</param>
    public BrokenRulesCollection(long code, string message, string domain = "App")
    {
        Add(code, message, domain);
    }

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="BrokenRulesCollection"/> с одним правилом.
    /// </summary>
    /// <param name="brokenRule">Нарушенное правило.</param>
    public BrokenRulesCollection(BrokenRule brokenRule)
    {
        Add(brokenRule);
    }

    /// <summary>
    /// Добавляет правило в коллекцию.
    /// </summary>
    /// <param name="message">Сообщение об ошибке.</param>
    /// <param name="code">Код правила.</param>
    public void Add(long code, string message, string domain = "App")
    {
        _brokenRules.Add(new BrokenRule(code, message, domain));
    }

    /// <summary>
    /// Добавляет правило в коллекцию.
    /// </summary>
    /// <param name="brokenRule">Нарушенное правило.</param>
    public void Add(BrokenRule brokenRule)
    {
        _brokenRules.Add(brokenRule ?? throw new ArgumentNullException(nameof(brokenRule)));
    }

    /// <summary>
    /// Преобразует коллекцию в ReadOnlyCollection.
    /// </summary>
    public ReadOnlyCollection<BrokenRule> ToReadOnlyCollection()
    {
        return new ReadOnlyCollection<BrokenRule>(_brokenRules);
    }

    /// <summary>
    /// Возвращает перечислитель для коллекции.
    /// </summary>
    public IEnumerator<BrokenRule> GetEnumerator()
    {
        return _brokenRules.GetEnumerator();
    }

    /// <summary>
    /// Возвращает перечислитель для коллекции.
    /// </summary>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
