using Domain;
using Domain.Entities;
using WebApp.Models.Responses;

namespace WebApp.Repositories.Interfaces.VkOrd.Creative
{
    /// <summary>
    /// Репозиторий для получения списка креативов VK ORD API
    /// </summary>
    public interface IGetPageCreativesRepository
    {
        Task<GetPageVkOrdResponse> Get(PageRequest pageRequest, CancellationToken cancellationToken);
    }
}
