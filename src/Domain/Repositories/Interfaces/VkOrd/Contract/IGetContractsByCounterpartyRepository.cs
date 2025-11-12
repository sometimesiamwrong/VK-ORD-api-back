using Domain.Entities.VkOrd;

namespace Domain.Repositories.Interfaces.VkOrd.Contract
{
    /// <summary>
    /// Репозиторий для получения контрактов по контрагенту VK ORD API
    /// </summary>
    public interface IGetContractsByCounterpartyRepository
    {
        Task<List<VkOrdContract>> Get(string counterpartyExternalId, CancellationToken cancellationToken);
    }
}
