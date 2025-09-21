using VkOrdApiWrapper.Models.Responses;

namespace VkOrdApiWrapper.Services.Interfaces
{
	public interface IDaDataService
	{
		Task<DaDataPartyShortResponse?> FindPartyByInnAsync(string inn, CancellationToken cancellationToken = default);
	}
}

