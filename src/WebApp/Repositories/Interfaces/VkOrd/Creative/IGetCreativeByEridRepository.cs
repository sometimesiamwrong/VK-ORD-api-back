using Domain.Entities;
using Domain.Entities.VkOrd;
using Domain.VkOrdApi.Creative;
using WebApp.Models.Responses;

namespace WebApp.Repositories.Interfaces.VkOrd.Creative
{
    /// <summary>
    /// Репозиторий для получения креатива VK ORD API по ERID
    /// </summary>
    public interface IGetCreativeByEridRepository
    {
        Task<VkOrdCreative> GetCreativeByErid(string erid, CancellationToken cancellationToken);
    }
}
