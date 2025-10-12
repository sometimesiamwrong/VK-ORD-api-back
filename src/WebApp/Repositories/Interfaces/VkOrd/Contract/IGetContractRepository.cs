using Domain.Entities.VkOrd;
using WebApp.Models.Responses;

namespace WebApp.Repositories.Interfaces.VkOrd.Contract
{
    /// <summary>
    /// Репозиторий для получения контракта VK ORD API
    /// </summary>
    public interface IGetContractRepository
    {
        Task<VkOrdContract> Get(string externalId, CancellationToken cancellationToken);
    }
}
