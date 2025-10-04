using MediatR;
using WebApp.Handlers.Requests;
using WebApp.Models.Responses;
using WebApp.Services.Interfaces;

namespace WebApp.Handlers
{
    public class CreateCreativeHandler : IRequestHandler<CreateCreativeRequestWrapper, CreateCreativeResponse>
    {
        private readonly IVkOrdService _vkOrdService;
        private readonly ILogger<CreateCreativeHandler> _logger;

        public CreateCreativeHandler(IVkOrdService vkOrdService, ILogger<CreateCreativeHandler> logger)
        {
            _vkOrdService = vkOrdService;
            _logger = logger;
        }

        public async Task<CreateCreativeResponse> Handle(CreateCreativeRequestWrapper request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Creating creative for user: {UserId}", request.UserId);

                var result = await _vkOrdService.CreateCreativeAsync(request.Request, request.UserId, request.Environment);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating creative");
                return new CreateCreativeResponse
                {
                    Success = false,
                    ErrorMessage = "Произошла ошибка при создании креатива"
                };
            }
        }
    }
}
