using Domain.Entities;
using Domain.VkOrdApi.Creative;
using WebApp.Models.Responses;

namespace WebApp.Repositories.Interfaces.VkOrd.Creative
{
    /// <summary>
    /// Репозиторий для получения креатива VK ORD API по ERID
    /// </summary>
    public interface IGetCreativeByEridRepository
    {
        Task<VkOrdApiCreativeV3Response> GetCreativeByErid(string erid, CancellationToken cancellationToken);
    }
}
