using Domain.Models.Responses;

namespace Domain.Repositories.Interfaces.VkOrd.Creative
{
    /// <summary>
    /// Репозиторий для получения списка креативов VK ORD API
    /// </summary>
    public interface IGetPageCreativesRepository
    {
        Task<GetPageVkOrdResponse> Get(PageRequest pageRequest, CancellationToken cancellationToken);
    }
}
