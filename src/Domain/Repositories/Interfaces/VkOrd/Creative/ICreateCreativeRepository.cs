using Domain.VkOrdApi.Creative;

namespace Domain.Repositories.Interfaces.VkOrd.Creative
{
    /// <summary>
    /// Репозиторий для создания креатива VK ORD API
    /// </summary>
    public interface ICreateCreativeRepository
    {
        Task<VkOrdApiCreativeV3RequestResponse> CreateCreative(string externalId, VkOrdApiCreativeV3Request request, CancellationToken cancellationToken);
    }
}
