using Domain;
using VkOrdApi.Media;
using WebApp.Models.Requests;
using WebApp.Services.Interfaces;

namespace WebApp.Services.Implementations.VkOrd
{
    /// <summary>
    /// Сервис для работы с VK ОРД
    /// </summary>
    public partial class VkOrdService : IVkOrdService
    {
        public Task UploadMedia(UploadMediaRequest request, CancellationToken cancellationToken)
        {
            return _mediaRepository.UploadMedia(request, cancellationToken);
        }

        public Task<VkOrdMediaInfoResponse> GetMedia(string externalId, CancellationToken cancellationToken)
        {
            return  _mediaRepository.GetMedia(externalId, cancellationToken);
        }

        public async Task<VkOrdMediaInfoListResponseDto> GetPageMedia(PageRequest pageRequest, CancellationToken cancellationToken)
        {
            var pageMediaListResponse = await _mediaRepository.GetPageMedia(pageRequest, cancellationToken);

            if (pageMediaListResponse?.ExternalIds != null)
            {
                var externalIds = pageMediaListResponse.ExternalIds;
                var totalItemsCount = pageMediaListResponse.TotalItemsCount;
                var responseLimit = pageMediaListResponse.Limit;

                _logger.LogInformation($"Found {externalIds.Count} creatives (total: {totalItemsCount}, limit: {responseLimit})");

                var media = new List<VkOrdMediaInfoResponse>();

                foreach (var externalId in externalIds)
                {
                    try
                    {
                        var mediaInfo = await _mediaRepository.GetMedia(externalId, cancellationToken);
                        if (mediaInfo is not null)
                        {
                            media.Add(mediaInfo);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error fetching creative {externalId}");
                    }
                }

                _logger.LogInformation($"Successfully fetched {media.Count} out of {externalIds.Count} media");

                return new VkOrdMediaInfoListResponseDto
                {
                    Data = media,
                    TotalItemsCount = totalItemsCount,
                    Limit = responseLimit
                };
            }

            return new VkOrdMediaInfoListResponseDto
            {
                Data = new List<VkOrdMediaInfoResponse>(),
                TotalItemsCount = 0,
                Limit = 0
            };
        }
    }
}
