using WebApp.Models.Responses;

namespace WebApp.Services.Interfaces;

/// <summary>
/// Сервис для работы с DaData API
/// </summary>
public interface IDaDataService
{
	/// <summary>
	/// Найти компанию или ИП по ИНН
	/// </summary>
	Task<DaDataPartyShortResponse?> FindPartyByInnAsync(string inn, CancellationToken cancellationToken);
}
