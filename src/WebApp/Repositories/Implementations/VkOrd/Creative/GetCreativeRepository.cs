using Domain.Data;
using Domain.Entities;
using Domain.Entities.Enums;
using Domain.Entities.VkOrd;
using Domain.VkOrdApi.Creative;
using Microsoft.OpenApi.Extensions;
using WebApp.Models.Responses;
using WebApp.Repositories.Interfaces.VkOrd.Creative;
using WebApp.Services.Interfaces;

namespace WebApp.Repositories.Implementations.VkOrd.Creative
{
    /// <summary>
    /// Репозиторий для получения креатива VK ORD API по external ID
    /// </summary>
    public class GetCreativeRepository : IGetCreativeRepository
    {
        private readonly IVkOrdApiClientFactory _vkOrdClientFactory;
        private readonly ICacheService _cacheService;
        private readonly AppDbContext _context;
        private readonly ILogger<GetCreativeRepository> _logger;

        public GetCreativeRepository(
            IVkOrdApiClientFactory vkOrdClientFactory,
            ICacheService cacheService,
            AppDbContext context,
            ILogger<GetCreativeRepository> logger)
        {
            _vkOrdClientFactory = vkOrdClientFactory;
            _cacheService = cacheService;
            _context = context;
            _logger = logger;
        }   

        public async Task<VkOrdApiCreativeV3Response> GetCreative(string externalId, CancellationToken cancellationToken)
        {
            var vkOrdClient = await _vkOrdClientFactory.CreateClient();
            var entity = await vkOrdClient.GetCreativeV3ByExternalId(externalId, cancellationToken);
            return entity;
        }
    }
}
