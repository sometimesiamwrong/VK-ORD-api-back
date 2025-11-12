using Domain.Entities.VkOrd;

namespace Domain.Repositories.Interfaces.VkOrd.Contract
{
    /// <summary>
    /// Репозиторий для получения контракта VK ORD API
    /// </summary>
    public interface IGetContractRepository
    {
        Task<VkOrdContract> Get(string externalId, CancellationToken cancellationToken, bool noCache = false);
    }
}
