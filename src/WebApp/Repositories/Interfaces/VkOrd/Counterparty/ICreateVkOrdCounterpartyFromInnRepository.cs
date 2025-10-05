using Domain.Entities;
using VkOrdApi.Person;
using WebApp.Models.Responses;

namespace WebApp.Repositories.Interfaces.VkOrd.Counterparty
{
    /// <summary>
    /// Репозиторий для создания контрагента VK ORD API по ИНН
    /// </summary>
    public interface ICreateVkOrdCounterpartyFromInnRepository
    {
        Task CreateCounterpartyFromInnAsync(string inn, List<VkOrdPersonRoles> types, DaDataPartyShortResponse daData, CancellationToken cancellationToken);
    }
}
