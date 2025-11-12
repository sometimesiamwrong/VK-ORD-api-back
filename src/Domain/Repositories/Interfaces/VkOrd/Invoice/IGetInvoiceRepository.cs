using Domain.Entities.VkOrd;

namespace Domain.Repositories.Interfaces.VkOrd.Invoice;

/// <summary>
/// Интерфейс репозитория для получения акта VK ORD API
/// </summary>
public interface IGetInvoiceRepository
{
    /// <summary>
    /// Получить акт по внешнему идентификатору
    /// </summary>
    /// <param name="externalId">Внешний идентификатор акта</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <param name="noCache">Игнорировать кэш и получить данные из API</param>
    /// <returns>Данные акта</returns>
    Task<VkOrdInvoice> Get(string externalId, CancellationToken cancellationToken, bool noCache = false);
}
