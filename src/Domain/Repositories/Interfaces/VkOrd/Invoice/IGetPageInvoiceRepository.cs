using Domain.VkOrdApi.Invoice;

namespace Domain.Repositories.Interfaces.VkOrd.Invoice;

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
    /// <param name="externalIds">Список внешних идентификаторов актов</param>
    /// <returns>Список внешних идентификаторов актов с метаданными</returns>
    Task<VkOrdApiInvoiceListResponse> Get(PageRequest pageRequest, CancellationToken cancellationToken, List<string>? externalIds = null);
}
