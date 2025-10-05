using Domain;
using Domain.Entities;
using VkOrdApi.Creative;
using WebApp.Models.Responses;
using WebApp.Repositories.Interfaces.VkOrd.Creative;
using WebApp.Services.Interfaces;

namespace WebApp.Repositories.Implementation.VkOrd.Creative
{
    /// <summary>
    /// Репозиторий для получения списка креативов VK ORD API
    /// </summary>
    public class GetAllCreativesRepository : IGetAllCreativesRepository
    {
        private readonly IVkOrdApiClientFactory _vkOrdClientFactory;
        private readonly ILogger<GetAllCreativesRepository> _logger;

        public GetAllCreativesRepository(
            IVkOrdApiClientFactory vkOrdClientFactory,
            ILogger<GetAllCreativesRepository> logger)
        {
            _vkOrdClientFactory = vkOrdClientFactory;
            _logger = logger;
        }

        public async Task<GetPageVkOrdResponse> GetAllCreatives(PageRequest pageRequest, CancellationToken cancellationToken)
        {
            var vkOrdClient = await _vkOrdClientFactory.CreateClient();

            _logger.LogInformation($"Fetching creatives using route: (offset: {pageRequest.Offset}, limit: {pageRequest.Limit})");

            var response = await vkOrdClient.GetCreativesV1(pageRequest, cancellationToken);

            return new GetPageVkOrdResponse
            {
                ExternalIds = response.ExternalIds,
                TotalItemsCount = response.TotalItemsCount,
                Limit = response.Limit
            };
        }
    }
}
