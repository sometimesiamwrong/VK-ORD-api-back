using MediatR;
using WebApp.Handlers.Requests;
using WebApp.Models.Requests;
using WebApp.Models.Responses;
using WebApp.Services.Interfaces;

namespace WebApp.Handlers
{
    public class CreateCounterpartyFromInnHandler : IRequestHandler<CreateCounterpartyFromInnRequest>
    {
        private readonly IVkOrdService _vkOrdService;
        private readonly ILogger<CreateCounterpartyFromInnHandler> _logger;

        public CreateCounterpartyFromInnHandler(IVkOrdService vkOrdService, ILogger<CreateCounterpartyFromInnHandler> logger)
        {
            _vkOrdService = vkOrdService;
            _logger = logger;
        }

        public Task Handle(CreateCounterpartyFromInnRequest request, CancellationToken cancellationToken)
        {
            return _vkOrdService.CreateCounterpartyFromInnAsync(request.Inn, request.Types, cancellationToken);
        }
    }
}
