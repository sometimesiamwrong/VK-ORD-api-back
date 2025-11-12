using Domain.Entities.Enums.VkOrd;
using Domain.Models.Responses;

namespace Domain.Repositories.Interfaces.VkOrd.Counterparty
{
    /// <summary>
    /// Репозиторий для создания контрагента VK ORD API по ИНН
    /// </summary>
    public interface ICreateVkOrdCounterpartyFromInnRepository
    {
        Task CreateCounterpartyFromInnAsync(string inn, List<VkOrdApiPersonRoles> types, DaDataPartyShortResponse daData, CancellationToken cancellationToken);
    }
}
