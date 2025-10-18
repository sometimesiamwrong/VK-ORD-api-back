using Domain.VkOrdApi.Invoice;

namespace WebApp.Repositories.Interfaces.VkOrd.Invoice;

/// <summary>
/// Интерфейс репозитория для получения заголовка акта VK ORD API
/// </summary>
public interface IGetInvoiceHeaderRepository
{
    /// <summary>
    /// Получить заголовок акта
    /// </summary>
    /// <param name="externalId">Внешний идентификатор акта</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task<VkOrdApiInvoiceHeaderResponse> GetHeader(string externalId, CancellationToken cancellationToken);
}
