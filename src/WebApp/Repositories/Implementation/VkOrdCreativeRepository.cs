using VkOrdApi.Services.Interfaces;
using VkOrdApi.Contract;
using WebApp.Models.Requests;
using WebApp.Models.Responses;
using WebApp.Repositories.Interfaces;
using Domain.Entities;
using System.Collections.Generic;
using System;

namespace WebApp.Repositories.Implementation
{
    /// <summary>
    /// Репозиторий для работы с креативами VK ORD API
    /// </summary>
    public class VkOrdCreativeRepository : IVkOrdCreativeRepository
    {
        private readonly IVkOrdApiClientFactory _vkOrdClientFactory;
        private readonly ILogger<VkOrdCreativeRepository> _logger;

        public VkOrdCreativeRepository(
            IVkOrdApiClientFactory vkOrdClientFactory,
            ILogger<VkOrdCreativeRepository> logger)
        {
            _vkOrdClientFactory = vkOrdClientFactory;
            _logger = logger;
        }

        #region Креативы

        public async Task<CreateCreativeResponse> CreateCreativeAsync(CreateCreativeRequest request, VkApiContext apiContext, CancellationToken cancellationToken = default)
        {
            var vkOrdClient = _vkOrdClientFactory.CreateClient(apiContext);

            var vkOrdCreative = new VkOrdCreative()
            {
                ExternalId = request.ExternalId,
                Name = request.Name,
                ContractExternalIds = request.ContractExternalIds,
                MediaExternalIds = request.MediaExternalIds,
                Form = request.Format.ToString(),
                TargetUrls = request.ContentUrls,
                Targeting = request.TargetAudience,
                KKTYCodes = request.KKTYCodes,
                PayType = VkCreativePayType.cpa.ToString(),
                Texts = new List<string> { request.Text },
                Flags = new List<string> { "native" }
            };

            _logger.LogInformation($"Creating creative with external_id: {vkOrdCreative.ExternalId} using route: {apiContext.Route}");

            var response = await vkOrdClient.CreateOrUpdateCreativeAsync(
                vkOrdCreative.ExternalId, vkOrdCreative, cancellationToken);

            if (response.IsSuccess)
            {
                var result = new CreateCreativeResponse
                {
                    Erid = response.Erid,
                    Success = true,
                };

                _logger.LogInformation($"Creative created successfully. ERID: {response.Erid}");
                return result;
            }
            else
            {
                _logger.LogError($"Failed to create creative: {response.Error}");
                return new CreateCreativeResponse
                {
                    ExternalId = vkOrdCreative.ExternalId,
                    Success = false,
                    ErrorMessage = response.Error
                };
            }
        }

        public async Task<CreateCreativeResponse> GetCreativeAsync(string externalId, VkApiContext apiContext, CancellationToken cancellationToken = default)
        {
            var vkOrdClient = _vkOrdClientFactory.CreateClient(apiContext);
            var response = await vkOrdClient.GetCreativeAsync(externalId, cancellationToken);

            if (response is not null)
            {
                var result = new CreateCreativeResponse
                {
                    ExternalId = externalId,
                    Erid = response.Erid,
                    Success = true
                };
                return result;
            }

            return new CreateCreativeResponse
            {
                Success = false,
                ErrorMessage = "Не удалось получить креатив",
                ExternalId = externalId
            };
        }

        public async Task<GetCreativesResponse> GetAllCreativesAsync(VkApiContext apiContext, CancellationToken cancellationToken = default, int? offset = null, int? limit = null)
        {
            var vkOrdClient = _vkOrdClientFactory.CreateClient(apiContext);

            _logger.LogInformation($"Fetching creatives using route: {apiContext.Route} (offset: {offset}, limit: {limit})");

            var response = await vkOrdClient.GetCreativesAsync(offset, limit, cancellationToken);

            if (response?.ExternalIds != null)
            {
                var externalIds = response.ExternalIds;
                var totalItemsCount = response.TotalItemsCount;
                var responseLimit = response.Limit;

                var creatives = new List<VkOrdCreative>();

                foreach (var externalId in externalIds)
                {
                    try
                    {
                        var creativeResponse = await vkOrdClient.GetCreativeAsync(externalId, cancellationToken);
                        if (creativeResponse is not null)
                        {
                            creatives.Add(creativeResponse);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error fetching creative {externalId}");
                    }
                }

                return new GetCreativesResponse
                {
                    Success = true,
                    Creatives = creatives,
                    TotalItemsCount = totalItemsCount,
                    Limit = responseLimit
                };
            }

            return new GetCreativesResponse
            {
                Success = false,
                ErrorMessage = "Не удалось получить список креативов"
            };
        }

        public async Task<CreativeResponse> GetCreativeByEridAsync(string erid, VkApiContext apiContext, CancellationToken cancellationToken = default)
        {
            var vkOrdClient = _vkOrdClientFactory.CreateClient(apiContext);
            var response = await vkOrdClient.GetCreativeByEridAsync(erid, cancellationToken);

            if (response == null)
            {
                return new CreativeResponse
                {
                    Success = false,
                    Message = "Пустой ответ VK ОРД",
                    ExternalId = string.Empty
                };
            }

            var externalId = response.Data?.ExternalId ?? string.Empty;
            return CreativeResponse.FromVkOrdResponse(response, externalId);
        }

        public async Task<VkOrdStatusResponse> GetCreativeStatusAsync(string externalId, VkApiContext apiContext, CancellationToken cancellationToken = default)
        {
            var vkOrdClient = _vkOrdClientFactory.CreateClient(apiContext);
            return await vkOrdClient.GetCreativeStatusAsync(externalId, cancellationToken);
        }

        public async Task<bool> DeleteCreativeAsync(string externalId, VkApiContext apiContext, CancellationToken cancellationToken = default)
        {
            var vkOrdClient = _vkOrdClientFactory.CreateClient(apiContext);
            var response = await vkOrdClient.DeleteCreativeAsync(externalId, cancellationToken);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<CreateCreativeResponse>> CreateBulkCreativesAsync(List<CreateCreativeRequest> requests, VkApiContext apiContext, CancellationToken cancellationToken = default)
        {
            var results = new List<CreateCreativeResponse>();
            foreach (var request in requests)
            {
                var result = await CreateCreativeAsync(request, apiContext, cancellationToken);
                results.Add(result);
            }
            return results;
        }

        #endregion
    }
}
