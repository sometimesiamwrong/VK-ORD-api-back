using Domain.VkOrdApi.Invoice;

namespace Domain.Repositories.Interfaces.VkOrd.Invoice;

/// <summary>
/// Интерфейс репозитория для добавления договоров в акт VK ORD API
/// </summary>
public interface IAddContractsToInvoiceRepository
{
    /// <summary>
    /// Добавить договоры в акт
    /// </summary>
    /// <param name="externalId">Внешний идентификатор акта</param>
    /// <param name="request">Данные договоров для добавления</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task AddContracts(string externalId, VkOrdApiAddContractsToInvoiceRequest request, CancellationToken cancellationToken);
}
