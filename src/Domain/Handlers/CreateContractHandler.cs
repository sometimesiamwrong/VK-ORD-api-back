using Domain.Handlers.Interfaces;
using Domain.Models.Requests;
using Domain.Services.Interfaces;
using MediatR;

namespace Domain.Handlers
{
    public class CreateContractHandler : ICreateContractHandler
    {
        private readonly IVkOrdService _vkOrdService;
        private readonly ILogger<CreateContractHandler> _logger;

        public CreateContractHandler(IVkOrdService vkOrdService, ILogger<CreateContractHandler> logger)
        {
            _vkOrdService = vkOrdService;
            _logger = logger;
        }

        public async Task<Unit> Handle(CreateContractRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling CreateContractRequest for ExternalId: {ExternalId}", request.ExternalId);
            
            await _vkOrdService.CreateOrUpdateContract(request, cancellationToken);
            
            return Unit.Value;
        }
    }
}
