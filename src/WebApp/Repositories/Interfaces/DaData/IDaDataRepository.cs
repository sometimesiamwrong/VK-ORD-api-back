using WebApp.Models.DaData;

namespace WebApp.Repositories.Interfaces.DaData
{
    /// <summary>
    /// Репозиторий для работы с DaData API
    /// </summary>
    public interface IDaDataRepository
    {
        /// <summary>
        /// Найти контрагента по ИНН
        /// </summary>
        Task<DaDataPartyResponse?> FindCounterpartyByInnAsync(string inn, CancellationToken cancellationToken);
    }
}
