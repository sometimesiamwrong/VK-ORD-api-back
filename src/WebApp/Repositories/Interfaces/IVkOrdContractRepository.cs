using Domain.Entities;
using VkOrdApi.Contract;
using WebApp.Models.Requests;
using WebApp.Models.Responses;

namespace WebApp.Repositories.Interfaces
{
    /// <summary>
    /// Репозиторий для работы с контрактами VK ORD API
    /// </summary>
    public interface IVkOrdContractRepository
    {
        // Контракты
        Task CreateOrUpdateContractAsync(string externalId, VkOrdCreateUpdateContractRequest request, CancellationToken cancellationToken);
        Task<ContractResponse> GetContractAsync(string externalId, VkApiContext apiContext, CancellationToken cancellationToken);
    }
}
