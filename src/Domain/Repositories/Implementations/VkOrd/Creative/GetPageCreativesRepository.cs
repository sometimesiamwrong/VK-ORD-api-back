using Domain.Models.Responses;
using Domain.Repositories.Interfaces.VkOrd.Creative;
using Domain.Services.Interfaces;
using Domain.VkOrdApi.Creative;

namespace Domain.Repositories.Implementations.VkOrd.Creative
{
    /// <summary>
    /// Репозиторий для получения списка креативов VK ORD API
    /// </summary>
    public class GetPageCreativesRepository : IGetPageCreativesRepository
    {
        private readonly IVkOrdApiClientFactory _vkOrdClientFactory;
        private readonly ILogger<GetPageCreativesRepository> _logger;

        public GetPageCreativesRepository(
            IVkOrdApiClientFactory vkOrdClientFactory,
            ILogger<GetPageCreativesRepository> logger)
        {
            _vkOrdClientFactory = vkOrdClientFactory;
            _logger = logger;
        }

        public async Task<GetPageVkOrdResponse> Get(PageRequest pageRequest, CancellationToken cancellationToken)
        {
            var vkOrdClient = await _vkOrdClientFactory.CreateClient();

            _logger.LogInformation($"Fetching creatives using route: (offset: {pageRequest.Offset}, limit: {pageRequest.Limit})");

            var data = new GetPageVkOrdResponse();

             if(pageRequest.NeedAll)
            {
                data.Limit = pageRequest.UnlimitedLimit;
                var response = new VkOrdApiCreativeListResponse();
                do
                {
                    response = await vkOrdClient.GetCreativesV1(
                        new PageRequest { 
                            Offset = response.ExternalIds.Count,  
                            NeedAll = true 
                        }, 
                        cancellationToken);

                    data.ExternalIds.AddRange(response.ExternalIds);
                    data.TotalItemsCount = response.TotalItemsCount;
                    data.Limit = response.Limit;

                }while(data.ExternalIds.Count < data.TotalItemsCount);
            }
            else
            {
                var response = await vkOrdClient.GetCreativesV1(pageRequest, cancellationToken);
                data.ExternalIds = response.ExternalIds;
                data.TotalItemsCount = response.TotalItemsCount;
                data.Limit = response.Limit;
            }

            return data;
        }
    }
}
