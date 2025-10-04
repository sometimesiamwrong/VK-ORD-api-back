using Domain.Entities;
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
        Task<UploadMediaResponse> UploadMediaAsync(UploadMediaRequest request, VkApiContext apiContext, CancellationToken cancellationToken);
        Task<GetMediaResponse> GetMediaAsync(string externalId, VkApiContext apiContext, CancellationToken cancellationToken);
        Task<bool> DeleteMediaAsync(string externalId, VkApiContext apiContext, CancellationToken cancellationToken);
    }
}
