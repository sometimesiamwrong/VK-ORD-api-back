using Domain.Models.Responses;
using Domain.Repositories.Interfaces.VkOrd.Contract;
using Domain.Services.Interfaces;
using Domain.VkOrdApi.Contract;

namespace Domain.Repositories.Implementations.VkOrd.Contract
{
    /// <summary>
    /// Репозиторий для получения списка контрактов VK ORD API
    /// </summary>
    public class GetPageContractRepository : IGetPageContractRepository
    {
        private readonly IVkOrdApiClientFactory _vkOrdClientFactory;

        public GetPageContractRepository(
            IVkOrdApiClientFactory vkOrdClientFactory)
        {
            _vkOrdClientFactory = vkOrdClientFactory;
        }

        public async Task<GetPageVkOrdResponse> Get(PageRequest pageRequest, CancellationToken cancellationToken)
        {
            var vkOrdClient = await _vkOrdClientFactory.CreateClient();

            var data = new GetPageVkOrdResponse();

            if(pageRequest.NeedAll)
            {
                data.Limit = pageRequest.UnlimitedLimit;
                var response = new VkOrdApiContractListResponse();
                do
                {
                    response = await vkOrdClient.GetContracts(
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
                var response = await vkOrdClient.GetContracts(pageRequest, cancellationToken);
                data.ExternalIds = response.ExternalIds;
                data.TotalItemsCount = response.TotalItemsCount;
                data.Limit = response.Limit;
            }

            return data;
        }
    }
}
