using Domain;
using Domain.Entities.Enums.VkOrd;
using Domain.Entities.VkOrd;
using Domain.Extensions;
using Domain.VkOrdApi.Contract;
using Microsoft.IdentityModel.Tokens;
using WebApp.Models.Requests;
using WebApp.Models.Responses;
using WebApp.Services.Interfaces;

namespace WebApp.Services.Implementations.VkOrd
{
    /// <summary>
    /// Сервис для работы с VK ОРД
    /// </summary>
    public partial class VkOrdService : IVkOrdService
    {
        public Task CreateOrUpdateContract(
            CreateContractRequest request,
            CancellationToken cancellationToken)
        {
            var vkOrdContract = new VkOrdApiCreateUpdateContractRequest
            {
                Serial = request.Serial,
                ClientExternalId = request.ClientExternalId,
                ContractorExternalId = request.ContractorExternalId,
                Type = VkOrdApiContractType.Service,
                Amount = request.PaySum.ToString(),
                Date = request.Date,
                DateEnd = request.DateEnd,
                Flags = new List<VkOrdApiContractFlag> { VkOrdApiContractFlag.VatIncluded },
                ActionType = VkOrdApiActionType.Other,
                ApiSubjectType = VkOrdApiSubjectType.Distribution
            };

            return _createContractRepository.CreateOrUpdateContract(request.ExternalId, vkOrdContract, cancellationToken);
        }

        public async Task<VkOrdContract> GetContract(string externalId, CancellationToken cancellationToken)
        {
            return await _getContractRepository.Get(externalId, cancellationToken);
        }

        public async Task<GetContractsDto> GetPageContract(PageRequest pageRequest, CancellationToken cancellationToken)
        {
            var pageContractListResponse = await _getPageContractRepository.Get(pageRequest, cancellationToken);

            if(pageContractListResponse?.ExternalIds.IsNullOrEmpty() ?? true)
            {
                return new GetContractsDto
                {
                    Data = new List<VkOrdContract>(),
                    TotalItemsCount = 0,
                    Limit = 0
                };
            }

            _logger.LogInformation($"VK ORD API response - ExternalIds count: {pageContractListResponse?.ExternalIds?.Count ?? 0}, TotalItemsCount: {pageContractListResponse?.TotalItemsCount}, Limit: {pageContractListResponse?.Limit}");

            var externalIds = pageContractListResponse!.ExternalIds;
            var totalItemsCount = pageContractListResponse.TotalItemsCount;
            var responseLimit = pageContractListResponse.Limit;

            _logger.LogInformation($"Found {externalIds.Count} counterparties (total: {totalItemsCount}, responseLimit: {responseLimit}), fetching full data for each");

            // Получаем полные данные для каждого контрагента последовательно
            var contracts = new List<VkOrdContract>();

            foreach (var externalId in externalIds)
            {
                var counterpartyResponse = await _getContractRepository.Get(externalId, cancellationToken);
                if (counterpartyResponse?.Data != null)
                {
                    contracts.Add(counterpartyResponse);
                }
            }

            _logger.LogInformation($"Successfully fetched {contracts.Count} out of {externalIds.Count} counterparties");

            return new GetContractsDto
            {
                Data = contracts,
                TotalItemsCount = totalItemsCount,
                Limit = responseLimit
            };
        }
    }
}
