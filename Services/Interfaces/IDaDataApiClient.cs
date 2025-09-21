using Refit;
using VkOrdApiWrapper.Models.DaData;

namespace VkOrdApiWrapper.Services.Interfaces
{
	public interface IDaDataApiClient
	{
		[Post("/suggestions/api/4_1/rs/findById/party")]
		Task<DaDataPartyResponse> FindByIdPartyAsync([Body] DaDataFindByIdRequest request);
	}
}

