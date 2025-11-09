using Domain.Entities.VkOrd;

namespace WebApp.Repositories.Interfaces.VkOrd.Counterparty
{
    /// <summary>
    /// Репозиторий для получения всех контрагентов VK ORD API из базы данных по logical account
    /// </summary>
    public interface IGetAllCounterpartyRepository
    {
        /// <summary>
        /// Получить все контрагенты из базы данных для текущего credential (logical account)
        /// </summary>
        Task<List<VkOrdCounterparty>> GetAll(CancellationToken cancellationToken);
    }
}

