using Domain.Models.Requests;
using Domain.Services.Interfaces;
using MediatR;

namespace Domain.Handlers
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
            return _vkOrdService.CreateCounterpartyFromInn(request.Inn, request.Types, cancellationToken);
        }
    }
}
