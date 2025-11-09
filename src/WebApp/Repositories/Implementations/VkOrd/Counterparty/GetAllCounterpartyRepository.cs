using Domain.Data;
using Domain.Entities;
using Domain.Entities.VkOrd;
using Microsoft.EntityFrameworkCore;
using WebApp.Repositories.Interfaces.VkOrd.Counterparty;
using WebApp.Services.Interfaces;

namespace WebApp.Repositories.Implementations.VkOrd.Counterparty
{
    /// <summary>
    /// Репозиторий для получения всех контрагентов VK ORD API из базы данных по logical account
    /// </summary>
    public class GetAllCounterpartyRepository : IGetAllCounterpartyRepository
    {
        private readonly IVkOrdApiClientFactory _vkOrdClientFactory;
        private readonly AppDbContext _context;

        public GetAllCounterpartyRepository(
            IVkOrdApiClientFactory vkOrdClientFactory,
            AppDbContext context)
        {
            _vkOrdClientFactory = vkOrdClientFactory;
            _context = context;
        }

        public async Task<List<VkOrdCounterparty>> GetAll(CancellationToken cancellationToken)
        {
            var vkOrdCredential = await _vkOrdClientFactory.GetVkOrdCredentialAsync();

            var counterparties = await _context.VkOrdCounterparties
                .AsNoTracking()
                .Where(x => x.LogicalAccountId == vkOrdCredential.LogicalAccountId && x.IsDeleted == false)
                .ToListAsync(cancellationToken);

            return counterparties;
        }
    }
}

