using Domain;
using Domain.Entities;
using VkOrdApi.Media;
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
        Task<VkOrdMediaInfoResponse> GetMedia(string externalId, CancellationToken cancellationToken);
        //Task<byte[]> GetMediaFile(string externalId, CancellationToken cancellationToken);
        Task<VkOrdMediaListResponse> GetPageMedia(PageRequest pageRequest, CancellationToken cancellationToken);
    }
}
