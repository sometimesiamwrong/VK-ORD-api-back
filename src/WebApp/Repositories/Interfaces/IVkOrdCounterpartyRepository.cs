using Domain.Entities;
using WebApp.Models.Requests;
using WebApp.Models.Responses;
using WebApp.Models.DaData;
using VkOrdApi.Person; // Assuming DaDataPartyShortResponse is here

namespace WebApp.Repositories.Interfaces
{
    /// <summary>
    /// Репозиторий для работы с контрагентами VK ORD API
    /// </summary>
    public interface IVkOrdCounterpartyRepository
    {
        // Контрагенты
        Task CreateCounterpartyFromInnAsync(string inn, List<VkOrdPersonRoles> types, CancellationToken cancellationToken);
        Task<GetCounterpartiesResponse> GetAllCounterpartiesAsync(VkApiContext apiContext, CancellationToken cancellationToken, int? offset = null, int? limit = null);
        Task<GetCounterpartyResponse> GetCounterpartyByIdAsync(string externalId, VkApiContext apiContext, CancellationToken cancellationToken);
    }
}
