using VkOrdApi.Contract;
using WebApp.Models.Responses;
using WebApp.Repositories.Interfaces.VkOrd.Contract;
using WebApp.Services.Interfaces;

namespace WebApp.Repositories.Implementation.VkOrd.Contract
{
    /// <summary>
    /// Репозиторий для получения контрактов VK ORD API
    /// </summary>
    public class GetContractRepository : IGetContractRepository
    {
        private readonly IVkOrdApiClientFactory _vkOrdClientFactory;

        public GetContractRepository(
            IVkOrdApiClientFactory vkOrdClientFactory)
        {
            _vkOrdClientFactory = vkOrdClientFactory;
        }

        public async Task<ContractResponse> GetContract(string externalId, CancellationToken cancellationToken)
        {
            var vkOrdClient = await _vkOrdClientFactory.CreateClient();

            var vkResponse = await vkOrdClient.GetContractByExternalId(externalId, cancellationToken);

            var contract = new VkOrdContract
            {
                CreateDate = vkResponse.CreateDate,
                Type = vkResponse.Type,
                ClientExternalId = vkResponse.ClientExternalId,
                ContractorExternalId = vkResponse.ContractorExternalId,
                ActionType = vkResponse.ActionType,
                SubjectType = vkResponse.SubjectType,
                Date = vkResponse.Date,
                DateEnd = vkResponse.DateEnd,
                Serial = vkResponse.Serial,
                Flags = vkResponse.Flags,
                ParentContractExternalId = vkResponse.ParentContractExternalId,
                Amount = vkResponse.Amount,
                HasAdditionalContracts = vkResponse.HasAdditionalContracts,
                Cid = vkResponse.Cid,
                LockedFields = vkResponse.LockedFields
            };

            return new ContractResponse
            {
                Data = contract,
                ExternalId = externalId
            };
        }
    }
}
