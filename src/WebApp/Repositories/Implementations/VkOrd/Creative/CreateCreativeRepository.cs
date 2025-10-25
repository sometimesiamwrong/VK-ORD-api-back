using Domain.BrokenRules;
using Domain.Data;
using Domain.Entities;
using Domain.Entities.Enums;
using Domain.Entities.VkOrd;
using Domain.Extensions;
using Domain.VkOrdApi.Creative;
using Microsoft.EntityFrameworkCore;
using WebApp.Repositories.Interfaces.VkOrd.Creative;
using WebApp.Services.Interfaces;

namespace WebApp.Repositories.Implementations.VkOrd.Creative
{
    /// <summary>
    /// Репозиторий для создания креатива VK ORD API
    /// </summary>
    public class CreateCreativeRepository : ICreateCreativeRepository
    {
        private readonly IVkOrdApiClientFactory _vkOrdClientFactory;
        private readonly ICacheService _cacheService;
        private readonly AppDbContext _context;
        private readonly ILogger<CreateCreativeRepository> _logger;

        public CreateCreativeRepository(
            IVkOrdApiClientFactory vkOrdClientFactory,
            ICacheService cacheService,
            AppDbContext context,
            ILogger<CreateCreativeRepository> logger)
        {
            _vkOrdClientFactory = vkOrdClientFactory;
            _cacheService = cacheService;
            _context = context;
            _logger = logger;
        }

        public async Task<VkOrdApiCreativeV3RequestResponse> CreateCreative(string externalId, VkOrdApiCreativeV3Request request, CancellationToken cancellationToken)
        {
            var vkOrdClient = await _vkOrdClientFactory.CreateClient();
            var vkOrdCredential = await _vkOrdClientFactory.GetVkOrdCredentialAsync();

            return await vkOrdClient.CreateOrUpdateCreativeV3(externalId, request, cancellationToken);
        }
    }
}
