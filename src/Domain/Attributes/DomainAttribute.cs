namespace Domain.Attributes;
    
/// <summary>
/// Атрибут для указания домена кода.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class DomainAttribute : Attribute
{
    /// <summary>
    /// <summary>
    /// Домен кода.
    /// </summary>
    public string Domain { get; }

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="DomainAttribute"/>.
    /// </summary>
    /// <param name="domain">Домен кода.</param>
    public DomainAttribute(string domain)
    {
        Domain = domain;
    }
}
