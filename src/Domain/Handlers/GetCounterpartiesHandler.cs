using Domain.Handlers.Requests;
using Domain.Models.Responses;
using Domain.Services.Interfaces;
using MediatR;

namespace Domain.Handlers
{
    public class GetCounterpartiesHandler : IRequestHandler<GetCounterpartiesRequest, GetCounterpartiesResponseDto>
    {
        private readonly IVkOrdService _vkOrdService;
        private readonly ILogger<GetCounterpartiesHandler> _logger;

        public GetCounterpartiesHandler(IVkOrdService vkOrdService, ILogger<GetCounterpartiesHandler> logger)
        {
            _vkOrdService = vkOrdService;
            _logger = logger;
        }

        public async Task<GetCounterpartiesResponseDto> Handle(GetCounterpartiesRequest request, CancellationToken cancellationToken)
        {
            return await _vkOrdService.GetPageCounterparties(request, cancellationToken); 
        }
    }
}
