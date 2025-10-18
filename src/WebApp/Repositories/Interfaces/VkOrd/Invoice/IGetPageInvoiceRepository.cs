using Domain;
using Domain.VkOrdApi.Invoice;

namespace WebApp.Repositories.Interfaces.VkOrd.Invoice;

/// <summary>
/// Интерфейс репозитория для получения списка актов VK ORD API с пагинацией
/// </summary>
public interface IGetPageInvoiceRepository
{
    /// <summary>
    /// Получить список актов с пагинацией
    /// </summary>
    /// <param name="pageRequest">Параметры пагинации</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Список внешних идентификаторов актов с метаданными</returns>
    Task<VkOrdApiInvoiceListResponse> Get(PageRequest pageRequest, CancellationToken cancellationToken);
}
