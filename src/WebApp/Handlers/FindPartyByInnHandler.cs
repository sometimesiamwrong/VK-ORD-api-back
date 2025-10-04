using MediatR;
using WebApp.Handlers.Requests;
using WebApp.Models.Responses;
using WebApp.Services.Interfaces;

namespace WebApp.Handlers
{
    public class FindPartyByInnHandler : IRequestHandler<FindPartyByInnQuery, DaDataPartyShortResponse?>
    {
        private readonly IDaDataService _daDataService;
        private readonly ILogger<FindPartyByInnHandler> _logger;

        public FindPartyByInnHandler(IDaDataService daDataService, ILogger<FindPartyByInnHandler> logger)
        {
            _daDataService = daDataService;
            _logger = logger;
        }

        public Task<DaDataPartyShortResponse?> Handle(FindPartyByInnQuery request, CancellationToken cancellationToken)
        {
            return _daDataService.FindPartyByInnAsync(request.Inn, cancellationToken);
        }
    }
}
