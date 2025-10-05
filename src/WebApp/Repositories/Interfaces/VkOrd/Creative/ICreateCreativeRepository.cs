
using VkOrdApi.Creative;
using WebApp.Models.Responses;

namespace WebApp.Repositories.Interfaces.VkOrd.Creative
{
    /// <summary>
    /// Репозиторий для создания креатива VK ORD API
    /// </summary>
    public interface ICreateCreativeRepository
    {
        Task<VkOrdCreativeV3RequestResponse> CreateCreative(string externalId, VkOrdCreativeV3Request request, CancellationToken cancellationToken);
    }
}
