using Domain.VkOrdApi.Creative;
using MediatR;
using WebApp.Handlers.Requests;
using WebApp.Models.Requests;
using WebApp.Models.Responses;
using WebApp.Services.Interfaces;

namespace WebApp.Handlers
{
    public class CreateCreativeHandler : IRequestHandler<CreateCreativeRequest, VkOrdApiCreativeV3RequestResponse>
    {
        private readonly IVkOrdService _vkOrdService;
        private readonly ILogger<CreateCreativeHandler> _logger;

        public CreateCreativeHandler(IVkOrdService vkOrdService, ILogger<CreateCreativeHandler> logger)
        {
            _vkOrdService = vkOrdService;
            _logger = logger;
        }

        public async Task<VkOrdApiCreativeV3RequestResponse> Handle(CreateCreativeRequest request, CancellationToken cancellationToken)
        {
            var result = await _vkOrdService.CreateCreative(request, cancellationToken);
            
            return result;
        }
    }
}
