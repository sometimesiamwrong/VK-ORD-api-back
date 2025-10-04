using VkOrdApi.Services.Interfaces;
using Refit;
using WebApp.Models.Requests;
using WebApp.Models.Responses;
using WebApp.Repositories.Interfaces;
using Domain.Entities;

namespace WebApp.Repositories.Implementation
{
    /// <summary>
    /// Репозиторий для работы с медиа файлами VK ORD API
    /// </summary>
    public class VkOrdMediaRepository : IVkOrdMediaRepository
    {
        private readonly IVkOrdApiClientFactory _vkOrdClientFactory;
        private readonly ILogger<VkOrdMediaRepository> _logger;

        public VkOrdMediaRepository(
            IVkOrdApiClientFactory vkOrdClientFactory,
            ILogger<VkOrdMediaRepository> logger)
        {
            _vkOrdClientFactory = vkOrdClientFactory;
            _logger = logger;
        }

        #region Медиа файлы

        public async Task<UploadMediaResponse> UploadMediaAsync(UploadMediaRequest request, VkApiContext apiContext, CancellationToken cancellationToken = default)
        {
            var vkOrdClient = _vkOrdClientFactory.CreateClient(apiContext);

            var streamPart = new StreamPart(request.FileStream, request.FileName, request.ContentType ?? "application/octet-stream");

            _logger.LogInformation($"Uploading media file with external_id: {request.ExternalId} using route: {apiContext.Route}");

            var response = await vkOrdClient.UploadMediaAsync(request.ExternalId, streamPart, cancellationToken);

            if (response.IsSuccess)
            {
                var result = new UploadMediaResponse
                {
                    Success = true,
                    ExternalId = request.ExternalId,
                    Erid = response.Erid,
                    Url = response.Data?.Url ?? string.Empty
                };

                _logger.LogInformation($"Media file uploaded successfully. ERID: {response.Erid}");
                return result;
            }
            else
            {
                _logger.LogError($"Failed to upload media file: {response.Error}");
                return new UploadMediaResponse
                {
                    Success = false,
                    ExternalId = request.ExternalId,
                    ErrorMessage = response.Error
                };
            }
        }

        public async Task<GetMediaResponse> GetMediaAsync(string externalId, VkApiContext apiContext, CancellationToken cancellationToken = default)
        {
            var vkOrdClient = _vkOrdClientFactory.CreateClient(apiContext);
            var response = await vkOrdClient.GetMediaAsync(externalId, cancellationToken);

            if (response.IsSuccess)
            {
                var result = new GetMediaResponse
                {
                    Success = true,
                    ExternalId = externalId,
                    Media = response.Data
                };
                return result;
            }
            else
            {
                return new GetMediaResponse
                {
                    Success = false,
                    ExternalId = externalId,
                    ErrorMessage = response.Error
                };
            }
        }

        public async Task<bool> DeleteMediaAsync(string externalId, VkApiContext apiContext, CancellationToken cancellationToken = default)
        {
            var vkOrdClient = _vkOrdClientFactory.CreateClient(apiContext);
            var response = await vkOrdClient.DeleteMediaAsync(externalId, cancellationToken);
            return response.IsSuccessStatusCode;
        }

        #endregion
    }
}
