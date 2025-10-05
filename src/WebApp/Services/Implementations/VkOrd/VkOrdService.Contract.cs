using Domain;
using Domain.Extensions;
using Microsoft.IdentityModel.Tokens;
using VkOrdApi.Contract;
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
            var vkOrdContract = new VkOrdCreateUpdateContractRequest
            {
                ClientExternalId = request.ClientExternalId,
                ContractorExternalId = request.ContractorExternalId,
                Type = VkOrdContractType.Service,
                Amount = request.PaySum.ToString(),
                Flags = new List<VkOrdContractFlag> { VkOrdContractFlag.VatIncluded },
                ActionType = VkOrdActionType.Other,
                SubjectType = VkOrdSubjectType.Distribution
            };

            return _createContractRepository.CreateOrUpdateContract(request.ExternalId, vkOrdContract, cancellationToken);
        }

        public Task<ContractResponse> GetContract(string externalId, CancellationToken cancellationToken)
        {
            return _getContractRepository.GetContract(externalId, cancellationToken);
        }

        public async Task<GetContractResponseDto> GetPageContract(PageRequest pageRequest, CancellationToken cancellationToken)
        {
            var pageContractListResponse = await _getPageContractRepository.Get(pageRequest, cancellationToken);

            if(pageContractListResponse?.ExternalIds.IsNullOrEmpty() ?? true)
            {
                return new GetContractResponseDto
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
                var counterpartyResponse = await _getContractRepository.GetContract(externalId, cancellationToken);
                if (counterpartyResponse?.Data != null)
                {
                    contracts.Add(counterpartyResponse.Data);
                }
            }

            _logger.LogInformation($"Successfully fetched {contracts.Count} out of {externalIds.Count} counterparties");

            return new GetContractResponseDto
            {
                Data = contracts,
                TotalItemsCount = totalItemsCount,
                Limit = responseLimit
            };
        }
    }
}
