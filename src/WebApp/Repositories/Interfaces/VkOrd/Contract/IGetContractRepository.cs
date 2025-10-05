using WebApp.Models.Responses;

namespace WebApp.Repositories.Interfaces.VkOrd.Contract
{
    /// <summary>
    /// Репозиторий для получения контракта VK ORD API
    /// </summary>
    public interface IGetContractRepository
    {
        Task<ContractResponse> GetContract(string externalId, CancellationToken cancellationToken);
    }
}
