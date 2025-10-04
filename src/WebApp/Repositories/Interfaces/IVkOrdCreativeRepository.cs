using Domain.Entities;
using WebApp.Models.Requests;
using WebApp.Models.Responses;

namespace WebApp.Repositories.Interfaces
{
    /// <summary>
    /// Репозиторий для работы с креативами VK ORD API
    /// </summary>
    public interface IVkOrdCreativeRepository
    {
        // Креативы
        Task<CreateCreativeResponse> CreateCreativeAsync(CreateCreativeRequest request, VkApiContext apiContext, CancellationToken cancellationToken);
        Task<CreateCreativeResponse> GetCreativeAsync(string externalId, VkApiContext apiContext, CancellationToken cancellationToken);
        Task<GetCreativesResponse> GetAllCreativesAsync(VkApiContext apiContext, CancellationToken cancellationToken, int? offset = null, int? limit = null);
        Task<CreativeResponse> GetCreativeByEridAsync(string erid, VkApiContext apiContext, CancellationToken cancellationToken);
        Task<Models.VkOrdStatusResponse> GetCreativeStatusAsync(string externalId, VkApiContext apiContext, CancellationToken cancellationToken);
        Task<bool> DeleteCreativeAsync(string externalId, VkApiContext apiContext, CancellationToken cancellationToken);
        Task<List<CreateCreativeResponse>> CreateBulkCreativesAsync(List<CreateCreativeRequest> requests, VkApiContext apiContext, CancellationToken cancellationToken);
    }
}
