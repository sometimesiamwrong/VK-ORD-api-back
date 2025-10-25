using Domain;
using Domain.Entities.VkOrd;
using Domain.VkOrdApi.Creative;
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
        public async Task<VkOrdCreative> CreateCreative(CreateCreativeRequest request, CancellationToken cancellationToken)
        {
            var vkOrdCreative = new VkOrdApiCreativeV3Request
            {
                PersonExternalId = request.PersonExternalId ?? null,
                ContractExternalIds = request.ContractExternalIds,
                Kktus = request.Kktus,
                Name = request.Name,
                Brand = request.Brand,
                Category = request.Category,
                Description = request.Description,
                ApiPayType = request.ApiPayType,
                Form = request.Type,
                Targeting = request.TargetAudience,
                TargetUrls = request.TargetUrls,
                Texts = request.Texts,
                MediaExternalIds = request.MediaExternalIds,
                Flags = request.Flags,
            };

            await _createCreativeRepository.CreateCreative(request.ExternalId, vkOrdCreative, cancellationToken);
            return await GetCreative(request.ExternalId, cancellationToken);
        }

        public async Task<VkOrdCreative> GetCreative(string externalId, CancellationToken cancellationToken)
        {
            return await _getCreativeRepository.Get(externalId, cancellationToken);
        }

        public async Task<GetCreativesResponse> GetPageCreatives(PageRequest pageRequest, CancellationToken cancellationToken)
        {
            var pageVkOrdResponse = await _getPageCreativesRepository.Get(pageRequest, cancellationToken);

            if (pageVkOrdResponse?.ExternalIds != null)
            {
                var externalIds = pageVkOrdResponse.ExternalIds;
                var totalItemsCount = pageVkOrdResponse.TotalItemsCount;
                var responseLimit = pageVkOrdResponse.Limit;

                _logger.LogInformation($"Found {externalIds.Count} creatives (total: {totalItemsCount}, limit: {responseLimit})");

                var creatives = new List<VkOrdCreative>();

                foreach (var externalId in externalIds)
                {
                    try
                    {
                        var creative = await _getCreativeRepository.Get(externalId, cancellationToken);
                        if (creative is not null)
                        {
                            creatives.Add(creative);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error fetching creative {externalId}");
                    }
                }

                _logger.LogInformation($"Successfully fetched {creatives.Count} out of {externalIds.Count} creatives");

                return new GetCreativesResponse
                {
                    Data = creatives,
                    TotalItemsCount = totalItemsCount,
                    Limit = responseLimit
                };
            }

            return new GetCreativesResponse
            {
                Data = new List<VkOrdCreative>(),
                TotalItemsCount = 0,
                Limit = 0,
            };
        }

        public async Task<VkOrdCreative> GetCreativeByErid(string erid, CancellationToken cancellationToken)
        {
            return await _getCreativeByEridRepository.GetCreativeByErid(erid, cancellationToken);
        }
    }
}
