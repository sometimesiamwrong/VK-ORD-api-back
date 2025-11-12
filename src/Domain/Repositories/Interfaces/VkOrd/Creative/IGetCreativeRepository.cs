using Domain.Entities.VkOrd;

namespace Domain.Repositories.Interfaces.VkOrd.Creative
{
    /// <summary>
    /// Репозиторий для получения креатива VK ORD API по external ID
    /// </summary>
    public interface IGetCreativeRepository
    {
        Task<VkOrdCreative> Get(string externalId, CancellationToken cancellationToken, bool noCache = false);
    }
}
