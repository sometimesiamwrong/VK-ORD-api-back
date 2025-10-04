using VkOrdApi.Services.Interfaces;
using VkOrdApi.Person;
using WebApp.Models.Requests;
using WebApp.Models.Responses;
using WebApp.Repositories.Interfaces;
using WebApp.Models.DaData; // For DaDataPartyShortResponse
using Domain.Entities;
using System.Collections.Generic;
using System;

namespace WebApp.Repositories.Implementation
{
    /// <summary>
    /// Репозиторий для работы с контрагентами VK ORD API
    /// </summary>
    public class VkOrdCounterpartyRepository : IVkOrdCounterpartyRepository
    {
        private readonly IVkOrdApiClientFactory _vkOrdClientFactory;
        private readonly ILogger<VkOrdCounterpartyRepository> _logger;

        public VkOrdCounterpartyRepository(
            IVkOrdApiClientFactory vkOrdClientFactory,
            ILogger<VkOrdCounterpartyRepository> logger)
        {
            _vkOrdClientFactory = vkOrdClientFactory;
            _logger = logger;
        }

        #region Контрагенты

        public async Task CreateCounterpartyFromInnAsync(string inn, List<VkOrdPersonRoles> roles, DaDataPartyShortResponse daData, CancellationToken cancellationToken = default)
        {
            VkOrdPersonType type;

            // dadata.Type:  LEGAL — юридическое лицо, INDIVIDUAL — индивидуальный предприниматель
            type = daData.Type == "LEGAL" ? VkOrdPersonType.Juridical : VkOrdPersonType.Ip;

            var name = daData.Value
                ?? daData.Name?.FullWithOpf
                ?? daData.Name?.Full
                ?? daData.Inn
                ?? string.Empty;

            // Map to VK ORD person schema
            var person = new VkOrdPersonResponse
            {
                Name = name,
                Roles = roles,
                RsUrl = null,
                JuridicalDetails = new VkOrdPersonJuridicalDetails
                {
                    Type = type,
                    ModelScheme = "russia",
                    Inn = daData.Inn,
                    Kpp = daData.Kpp,
                    Phone = daData.Phone,
                    ForeignEpaymentMethod = null,
                    ForeignRegistrationNumber = null,
                    ForeignInn = null,
                    ForeignOksmCountryCode = null
                }
            };

            var externalId = daData.Inn ?? inn;
            var vkOrdClient = _vkOrdClientFactory.CreateClient(apiContext);

            await vkOrdClient.CreateOrUpdatePersonAsync(externalId, person, cancellationToken);
        }

        public async Task<GetCounterpartiesResponse> GetAllCounterpartiesAsync(VkApiContext apiContext, CancellationToken cancellationToken = default, int? offset = null, int? limit = null)
        {
            var vkOrdClient = _vkOrdClientFactory.CreateClient(apiContext);

            _logger.LogInformation($"Fetching counterparties using route: {apiContext.Route} (offset: {offset}, limit: {limit})");

            var response = await vkOrdClient.GetPersonsAsync(offset, limit, cancellationToken);

            _logger.LogInformation($"VK ORD API response - ExternalIds count: {response?.ExternalIds?.Count ?? 0}, TotalItemsCount: {response?.TotalItemsCount}, Limit: {response?.Limit}");

            if (response?.ExternalIds != null)
            {
                var externalIds = response.ExternalIds;
                var totalItemsCount = response.TotalItemsCount;
                var responseLimit = response.Limit;

                _logger.LogInformation($"Found {externalIds.Count} counterparties (total: {totalItemsCount}, responseLimit: {responseLimit}), fetching full data for each");

                // Получаем полные данные для каждого контрагента последовательно
                var counterparties = new List<VkOrdPersonResponse>();

                foreach (var externalId in externalIds)
                {
                    var counterpartyResponse = await GetCounterpartyByIdAsync(externalId, apiContext, cancellationToken);
                    if (counterpartyResponse.Success && counterpartyResponse.Person != null)
                    {
                        counterparties.Add(counterpartyResponse.Person);
                    }
                }

                _logger.LogInformation($"Successfully fetched {counterparties.Count} out of {externalIds.Count} counterparties");

                return new GetCounterpartiesResponse
                {
                    Success = true,
                    Counterparties = counterparties,
                    TotalItemsCount = totalItemsCount,
                    Limit = responseLimit
                };
            }
            else
            {
                _logger.LogError("Failed to fetch counterparties: response is null or ExternalIds is null");
                return new GetCounterpartiesResponse
                {
                    Success = false,
                    ErrorMessage = "Не удалось получить список контрагентов"
                };
            }
        }

        public async Task<GetCounterpartyResponse> GetCounterpartyByIdAsync(string externalId, VkApiContext apiContext, CancellationToken cancellationToken = default)
        {
            var vkOrdClient = _vkOrdClientFactory.CreateClient(apiContext);
            _logger.LogInformation($"Fetching counterparty {externalId} using route: {apiContext.Route}");

            var person = await vkOrdClient.GetPersonAsync(externalId, cancellationToken);

            if (person != null)
            {
                return new GetCounterpartyResponse
                {
                    Success = true,
                    ExternalId = externalId,
                    Person = person
                };
            }
            else
            {
                _logger.LogError($"Failed to fetch counterparty {externalId}: person is null");
                return new GetCounterpartyResponse
                {
                    Success = false,
                    ExternalId = externalId,
                    ErrorMessage = "Не удалось получить контрагента"
                };
            }
        }

        #endregion
    }
}
