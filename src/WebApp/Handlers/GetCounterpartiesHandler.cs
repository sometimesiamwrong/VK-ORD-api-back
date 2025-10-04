using MediatR;
using WebApp.Handlers.Requests;
using WebApp.Models.Responses;
using WebApp.Services.Interfaces;

namespace WebApp.Handlers
{
    public class GetCounterpartiesHandler : IRequestHandler<GetCounterpartiesRequest, GetCounterpartiesResponse>
    {
        private readonly IVkOrdService _vkOrdService;
        private readonly ILogger<GetCounterpartiesHandler> _logger;

        public GetCounterpartiesHandler(IVkOrdService vkOrdService, ILogger<GetCounterpartiesHandler> logger)
        {
            _vkOrdService = vkOrdService;
            _logger = logger;
        }

        public async Task<GetCounterpartiesResponse> Handle(GetCounterpartiesRequest request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Getting counterparties for user: {UserId}", request.UserId);

                var result = await _vkOrdService.GetAllCounterpartiesAsync(request.UserId, request.Environment, request.Offset, request.Limit);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting counterparties");
                return new GetCounterpartiesResponse
                {
                    Success = false,
                    ErrorMessage = "Произошла ошибка при получении контрагентов"
                };
            }
        }
    }
}
