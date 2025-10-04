using WebApp.Handlers.Interfaces;
using WebApp.Handlers.Requests;
using WebApp.Models.Responses;
using WebApp.Services.Interfaces;

namespace WebApp.Handlers
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

        public async Task<CreateContractResponse> Handle(CreateContractRequestWrapper request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Creating contract for user: {UserId}", request.UserId);

                var result = await _vkOrdService.CreateOrUpdateContractAsync(request.Request, request.UserId, request.Environment);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating contract");
                return new CreateContractResponse
                {
                    Success = false,
                    ErrorMessage = "Произошла ошибка при создании контракта"
                };
            }
        }
    }
}
