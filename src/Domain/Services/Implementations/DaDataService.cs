using Domain.Models.Responses;
using Domain.Repositories.Interfaces.DaData;
using Domain.Services.Interfaces;

namespace Domain.Services.Implementations
{
	public class DaDataService : IDaDataService
	{
		private readonly IDaDataRepository _daDataRepository;
		private readonly ILogger<DaDataService> _logger;

		public DaDataService(IDaDataRepository daDataRepository, ILogger<DaDataService> logger)
		{
			_daDataRepository = daDataRepository;
			_logger = logger;
		}

		public async Task<DaDataPartyShortResponse?> FindPartyByInnAsync(string inn, CancellationToken cancellationToken)
		{
			if (string.IsNullOrWhiteSpace(inn)) return null;

			var counterparty = await _daDataRepository.FindCounterpartyByInnAsync(inn, cancellationToken);
			if (counterparty == null) 
			{
				return null;
			}

			return counterparty;
		}
	}
}

