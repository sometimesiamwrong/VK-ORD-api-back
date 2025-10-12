using MediatR;
using WebApp.Handlers.Requests;
using WebApp.Models.Responses;
using WebApp.Services.Interfaces;

namespace WebApp.Handlers
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
            var result = await _vkOrdService.GetPageCounterparties(request.PageRequest, cancellationToken); 
            await _vkOrdService.GetPageCounterparties(request.PageRequest, cancellationToken);
            return result;
        }
    }
}
