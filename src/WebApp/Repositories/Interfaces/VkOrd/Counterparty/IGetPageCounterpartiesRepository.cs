using Domain;
using WebApp.Models.Responses;

namespace WebApp.Repositories.Interfaces.VkOrd.Counterparty
{
    /// <summary>
    /// Репозиторий для получения списка контрагентов VK ORD API
    /// </summary>
    public interface IGetPageCounterpartiesRepository
    {
        Task<GetPageVkOrdResponse> Get(PageRequest pageRequest, CancellationToken cancellationToken);
    }
}
