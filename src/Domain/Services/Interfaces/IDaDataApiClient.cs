using Domain.Models.DaData;
using Refit;

namespace Domain.Services.Interfaces;

/// <summary>
/// Клиент для работы с DaData API
/// </summary>
public interface IDaDataApiClient
{
	[Post("/suggestions/api/4_1/rs/findById/party")]
	Task<DaDataPartyResponse> FindByIdPartyAsync([Body] DaDataFindByIdRequest request);
}

