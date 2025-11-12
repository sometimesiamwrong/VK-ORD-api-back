using Domain.Entities.VkOrd;

namespace Domain.Repositories.Interfaces.VkOrd.Counterparty
{
    /// <summary>
    /// Репозиторий для получения контрагента VK ORD API по ID
    /// </summary>
    public interface IGetCounterpartyByIdRepository
    {
        Task<VkOrdCounterparty> Get(string externalId, CancellationToken cancellationToken, bool noCache = false);
    }
}
