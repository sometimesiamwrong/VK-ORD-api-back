using Refit;
using WebApp.Models.DaData;

namespace WebApp.Services.Interfaces;

/// <summary>
/// Клиент для работы с DaData API
/// </summary>
public interface IDaDataApiClient
{
	[Post("/suggestions/api/4_1/rs/findById/party")]
	Task<DaDataPartyResponse> FindByIdPartyAsync([Body] DaDataFindByIdRequest request);
}

