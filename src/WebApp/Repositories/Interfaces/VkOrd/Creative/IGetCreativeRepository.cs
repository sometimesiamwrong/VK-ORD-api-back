using Domain.Entities;
using Domain.Entities.VkOrd;
using Domain.VkOrdApi.Creative;
using WebApp.Models.Responses;

namespace WebApp.Repositories.Interfaces.VkOrd.Creative
{
    /// <summary>
    /// Репозиторий для получения креатива VK ORD API по external ID
    /// </summary>
    public interface IGetCreativeRepository
    {
        Task<VkOrdCreative> Get(string externalId, CancellationToken cancellationToken);
    }
}
