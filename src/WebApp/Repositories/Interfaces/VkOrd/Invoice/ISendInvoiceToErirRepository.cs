namespace WebApp.Repositories.Interfaces.VkOrd.Invoice;

/// <summary>
/// Интерфейс репозитория для отправки акта в ЕРИР
/// </summary>
public interface ISendInvoiceToErirRepository
{
    /// <summary>
    /// Отправить акт в ЕРИР
    /// </summary>
    /// <param name="externalId">Внешний идентификатор акта</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task Send(string externalId, CancellationToken cancellationToken);
}
