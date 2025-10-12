using Domain;
using Domain.Entities;
using Domain.VkOrdApi.Media;
using WebApp.Models.Requests;
using WebApp.Models.Responses;

namespace WebApp.Repositories.Interfaces
{
    /// <summary>
    /// Репозиторий для работы с медиа файлами VK ORD API
    /// </summary>
    public interface IVkOrdMediaRepository
    {
        // Медиа файлы
        Task<string> UploadMedia(UploadMediaRequest request, CancellationToken cancellationToken);
        Task<VkOrdApiMediaInfoResponse> GetMedia(string externalId, CancellationToken cancellationToken);
        //Task<byte[]> GetMediaFile(string externalId, CancellationToken cancellationToken);
        Task<VkOrdApiMediaListResponse> GetPageMedia(PageRequest pageRequest, CancellationToken cancellationToken);
    }
}
