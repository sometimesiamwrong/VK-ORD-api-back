using Domain;
using Domain.BrokenRules;
using Domain.Entities.Enums.VkOrd;
using Domain.Entities.VkOrd;
using Domain.Extensions;
using Domain.VkOrdApi.Person;
using Microsoft.IdentityModel.Tokens;
using WebApp.Models.Responses;
using WebApp.Services.Interfaces;

namespace WebApp.Services.Implementations.VkOrd
{
    /// <summary>
    /// Сервис для работы с VK ОРД
    /// </summary>
    public partial class VkOrdService : IVkOrdService
    {
        public async Task CreateCounterpartyFromInn(string inn, List<VkOrdApiPersonRoles> roles, CancellationToken cancellationToken)
        {
            var dadata = await _daDataService.FindPartyByInnAsync(inn, cancellationToken);
            if (dadata == null)
            {
                throw BrokenRuleCodes.CounterpartyNotFound.AsExn("Контрагент по ИНН не найден в DaData");
            }
            
            VkOrdApiPersonType type;

            // dadata.Type:  LEGAL — юридическое лицо, INDIVIDUAL — индивидуальный предприниматель
            type = dadata.Type == "LEGAL" ? VkOrdApiPersonType.Juridical : VkOrdApiPersonType.Ip;

            var name = dadata.Value
                ?? dadata.Name?.FullWithOpf
                ?? dadata.Name?.Full
                ?? dadata.Inn
                ?? string.Empty;

            // Map to VK ORD person schema
            var person = new VkOrdApiPersonResponse
            {
                Name = name,
                Roles = roles,
                RsUrl = null,
                JuridicalDetails = new VkOrdApiPersonJuridicalDetails
                {
                    Type = type,
                    ModelScheme = "russia",
                    Inn = dadata.Inn,
                    Kpp = dadata.Kpp,
                    Phone = dadata.Phone,
                    ForeignEpaymentMethod = null,
                    ForeignRegistrationNumber = null,
                    ForeignInn = null,
                    ForeignOksmCountryCode = null
                }
            };

            var externalId = dadata.Inn ?? inn;

            await _createCounterpartyRepository.Create(externalId, person, cancellationToken);
        }

        public Task CreateCounterparty(string externalId, VkOrdApiPersonResponse apiPerson, CancellationToken cancellationToken)
        {
            return _createCounterpartyRepository.Create(externalId, apiPerson, cancellationToken);
        }


        public async Task<GetCounterpartiesResponseDto> GetPageCounterparties(PageRequest pageRequest, CancellationToken cancellationToken)
        {
            var pageContractListResponse = await _getPageCounterpartiesRepository.Get(pageRequest, cancellationToken);

            if(pageContractListResponse?.ExternalIds.IsNullOrEmpty() ?? true)
            {
                return new GetCounterpartiesResponseDto
                {
                    Data = new List<VkOrdApiPersonResponse>(),
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
            var counterparties = new List<VkOrdApiPersonResponse>();

            foreach (var externalId in externalIds)
            {
                var counterpartyResponse = await _getCounterpartyByIdRepository.Get(externalId, cancellationToken);
                if (counterpartyResponse?.Data != null)
                {
                    counterparties.Add(counterpartyResponse.Data);
                }
            }

            _logger.LogInformation($"Successfully fetched {counterparties.Count} out of {externalIds.Count} counterparties");

            return new GetCounterpartiesResponseDto
            {
                Data = counterparties,
                TotalItemsCount = totalItemsCount,
                Limit = responseLimit
            };
        }

        public Task<VkOrdCounterparty> GetCounterpartyById(string externalId, CancellationToken cancellationToken)
        {
            return _getCounterpartyByIdRepository.Get(externalId, cancellationToken);
        }
    }
}
