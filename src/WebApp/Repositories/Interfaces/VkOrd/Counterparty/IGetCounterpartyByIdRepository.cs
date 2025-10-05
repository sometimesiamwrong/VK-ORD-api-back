using WebApp.Models.Responses;

namespace WebApp.Repositories.Interfaces.VkOrd.Counterparty
{
    /// <summary>
    /// Репозиторий для получения контрагента VK ORD API по ID
    /// </summary>
    public interface IGetCounterpartyByIdRepository
    {
        Task<GetCounterpartyResponse?> GetCounterpartyByIdAsync(string externalId, CancellationToken cancellationToken);
    }
}
