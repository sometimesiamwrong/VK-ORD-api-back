using Domain.Entities;
using Domain.Entities.Enums.VkOrd;
using WebApp.Models.Responses;

namespace WebApp.Repositories.Interfaces.VkOrd.Counterparty
{
    /// <summary>
    /// Репозиторий для создания контрагента VK ORD API по ИНН
    /// </summary>
    public interface ICreateVkOrdCounterpartyFromInnRepository
    {
        Task CreateCounterpartyFromInnAsync(string inn, List<VkOrdApiPersonRoles> types, DaDataPartyShortResponse daData, CancellationToken cancellationToken);
    }
}
