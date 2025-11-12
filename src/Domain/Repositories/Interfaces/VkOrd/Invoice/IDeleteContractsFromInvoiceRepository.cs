using Domain.VkOrdApi.Invoice;

namespace Domain.Repositories.Interfaces.VkOrd.Invoice;

/// <summary>
/// Интерфейс репозитория для удаления договоров из акта VK ORD API
/// </summary>
public interface IDeleteContractsFromInvoiceRepository
{
    /// <summary>
    /// Удалить договоры из акта
    /// </summary>
    /// <param name="externalId">Внешний идентификатор акта</param>
    /// <param name="request">Данные договоров для удаления</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task DeleteContracts(string externalId, VkOrdApiDeleteContractsFromInvoiceRequest request, CancellationToken cancellationToken);
}
