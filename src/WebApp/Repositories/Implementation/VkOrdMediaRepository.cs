using Refit;
using WebApp.Models.Requests;
using WebApp.Repositories.Interfaces;
using WebApp.Services.Interfaces;
using Domain;
using VkOrdApi.Media;

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

        public async Task<string> UploadMedia(UploadMediaRequest request, CancellationToken cancellationToken)
        {
            var vkOrdClient = await _vkOrdClientFactory.CreateClient();

            var streamPart = new StreamPart(request.FileStream, request.FileName, request.ContentType ?? "application/octet-stream");

            _logger.LogInformation($"Uploading media file with external_id: {request.ExternalId}");

            await vkOrdClient.UploadMedia(request.ExternalId, streamPart, cancellationToken);
            
            return request.ExternalId;
        }

        public async Task<VkOrdMediaInfoResponse> GetMedia(string externalId, CancellationToken cancellationToken)
        {
            var vkOrdClient = await _vkOrdClientFactory.CreateClient();
            return await vkOrdClient.GetMediaInfo(externalId, cancellationToken);
        }

        //public async Task<byte[]> GetMediaFile(string externalId, CancellationToken cancellationToken)
        //{
        //    var vkOrdClient = await _vkOrdClientFactory.CreateClient();
        //    return await vkOrdClient.GetMediaFile(externalId, cancellationToken);
//
        //    //TODO: Скачивать на стороне клиента
        //}

        public async Task<VkOrdMediaListResponse> GetPageMedia(PageRequest pageRequest, CancellationToken cancellationToken)
        {
            var vkOrdClient = await _vkOrdClientFactory.CreateClient();
            return await vkOrdClient.GetPageMedia(pageRequest, cancellationToken);
        }
    }
}
