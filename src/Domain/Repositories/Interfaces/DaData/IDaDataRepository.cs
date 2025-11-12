using Domain.Models.Responses;

namespace Domain.Repositories.Interfaces.DaData
{
    /// <summary>
    /// Репозиторий для работы с DaData API
    /// </summary>
    public interface IDaDataRepository
    {
        /// <summary>
        /// Найти контрагента по ИНН
        /// </summary>
        Task<DaDataPartyShortResponse?> FindCounterpartyByInnAsync(string inn, CancellationToken cancellationToken);
    }
}
