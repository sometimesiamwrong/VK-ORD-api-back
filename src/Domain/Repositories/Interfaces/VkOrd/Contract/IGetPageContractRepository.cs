using Domain.Models.Responses;

namespace Domain.Repositories.Interfaces.VkOrd.Contract
{
    /// <summary>
    /// Репозиторий для получения списка контрактов VK ORD API
    /// </summary>
    public interface IGetPageContractRepository
    {
        Task<GetPageVkOrdResponse> Get(PageRequest pageRequest, CancellationToken cancellationToken);
    }
}
