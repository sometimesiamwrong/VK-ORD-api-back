using Domain.Entities.VkOrd;
using Domain.Models.Requests;
using Domain.Services.Interfaces;
using MediatR;

namespace Domain.Handlers
{
    public class CreateCreativeHandler : IRequestHandler<CreateCreativeRequest, VkOrdCreative>
    {
        private readonly IVkOrdService _vkOrdService;
        private readonly ILogger<CreateCreativeHandler> _logger;

        public CreateCreativeHandler(IVkOrdService vkOrdService, ILogger<CreateCreativeHandler> logger)
        {
            _vkOrdService = vkOrdService;
            _logger = logger;
        }

        public async Task<VkOrdCreative> Handle(CreateCreativeRequest request, CancellationToken cancellationToken)
        {
            var result = await _vkOrdService.CreateCreative(request, cancellationToken);
            
            return  result;
        }
    }
}
