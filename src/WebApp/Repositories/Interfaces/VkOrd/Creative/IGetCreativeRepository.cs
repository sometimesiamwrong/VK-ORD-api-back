using Domain.Entities;
using VkOrdApi.Creative;
using WebApp.Models.Responses;

namespace WebApp.Repositories.Interfaces.VkOrd.Creative
{
    /// <summary>
    /// Репозиторий для получения креатива VK ORD API по external ID
    /// </summary>
    public interface IGetCreativeRepository
    {
        Task<VkOrdCreativeV3Response> GetCreative(string externalId, CancellationToken cancellationToken);
    }
}
