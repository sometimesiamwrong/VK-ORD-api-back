using Domain.Models.Responses;

namespace Domain.Repositories.Interfaces.VkOrd.Counterparty
{
    /// <summary>
    /// Репозиторий для получения списка контрагентов VK ORD API
    /// </summary>
    public interface IGetPageCounterpartiesRepository
    {
        Task<GetPageVkOrdResponse> Get(PageRequest pageRequest, CancellationToken cancellationToken);
    }
}
