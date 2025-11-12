using Domain.VkOrdApi.Invoice;

namespace Domain.Repositories.Interfaces.VkOrd.Invoice;

/// <summary>
/// Интерфейс репозитория для создания/обновления заголовка акта VK ORD API
/// </summary>
public interface ICreateOrUpdateInvoiceHeaderRepository
{
    /// <summary>
    /// Создать или обновить заголовок акта
    /// </summary>
    /// <param name="externalId">Внешний идентификатор акта</param>
    /// <param name="request">Данные заголовка акта</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task CreateOrUpdateHeader(string externalId, VkOrdApiInvoiceHeaderRequest request, CancellationToken cancellationToken);
}
