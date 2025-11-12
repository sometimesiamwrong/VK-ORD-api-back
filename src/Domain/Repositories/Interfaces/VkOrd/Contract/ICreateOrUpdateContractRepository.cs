using Domain.VkOrdApi.Contract;

namespace Domain.Repositories.Interfaces.VkOrd.Contract
{
    /// <summary>
    /// Репозиторий для создания/обновления контрактов VK ORD API
    /// </summary>
    public interface ICreateOrUpdateContractRepository
    {
        Task CreateOrUpdateContract(string externalId, VkOrdApiCreateUpdateContractRequest request, CancellationToken cancellationToken);
    }
}
