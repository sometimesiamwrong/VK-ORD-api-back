using Domain.Entities.VkOrd;

namespace WebApp.Repositories.Interfaces.VkOrd.Invoice;

/// <summary>
/// Интерфейс репозитория для создания/обновления актов VK ORD API
/// </summary>
public interface ICreateOrUpdateInvoiceRepository
{
    /// <summary>
    /// Создать или обновить акт
    /// </summary>
    /// <param name="externalId">Внешний идентификатор акта</param>
    /// <param name="request">Данные акта</param>
    /// <param name="isDraft">Является ли черновиком</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Созданный/обновленный акт</returns>
    Task<VkOrdInvoice> CreateOrUpdateInvoice(
        string externalId, 
        WebApp.Models.Requests.CreateOrUpdateInvoiceRequest request,
        bool isDraft,
        CancellationToken cancellationToken);
}
