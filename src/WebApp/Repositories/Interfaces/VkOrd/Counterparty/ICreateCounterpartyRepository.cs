using Domain.VkOrdApi.Person;

namespace WebApp.Repositories.Interfaces.VkOrd.Counterparty
{
    /// <summary>
    /// Репозиторий для создания контрагента VK ORD API по ИНН
    /// </summary>
    public interface ICreateCounterpartyRepository
    {
        Task Create(string externalId, VkOrdApiPersonResponse apiPerson, CancellationToken cancellationToken);
    }
}
