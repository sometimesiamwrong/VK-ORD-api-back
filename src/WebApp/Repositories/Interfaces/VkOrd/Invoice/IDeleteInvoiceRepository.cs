namespace WebApp.Repositories.Interfaces.VkOrd.Invoice;

/// <summary>
/// Интерфейс репозитория для удаления акта VK ORD API
/// </summary>
public interface IDeleteInvoiceRepository
{
    /// <summary>
    /// Удалить акт по внешнему идентификатору
    /// </summary>
    /// <param name="externalId">Внешний идентификатор акта</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task Delete(string externalId, CancellationToken cancellationToken);
}
