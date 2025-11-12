using Domain.Entities.VkOrd;

namespace Domain.Repositories.Interfaces.VkOrd.Creative
{
    /// <summary>
    /// Репозиторий для получения креатива VK ORD API по ERID
    /// </summary>
    public interface IGetCreativeByEridRepository
    {
        Task<VkOrdCreative> GetCreativeByErid(string erid, CancellationToken cancellationToken);
    }
}
