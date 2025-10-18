using Domain.Data;
using Domain.Entities;
using Domain.Entities.Enums;
using Domain.Entities.VkOrd;
using Domain.Extensions;
using Microsoft.EntityFrameworkCore;
using WebApp.Repositories.Interfaces.VkOrd.Invoice;
using WebApp.Services.Interfaces;

namespace WebApp.Repositories.Implementations.VkOrd.Invoice;

/// <summary>
/// Репозиторий для удаления акта VK ORD API
/// </summary>
public class DeleteInvoiceRepository : IDeleteInvoiceRepository
{
    private readonly IVkOrdApiClientFactory _vkOrdClientFactory;
    private readonly ICacheService _cacheService;
    private readonly AppDbContext _context;
    private readonly ILogger<DeleteInvoiceRepository> _logger;

    public DeleteInvoiceRepository(
        IVkOrdApiClientFactory vkOrdClientFactory,
        ICacheService cacheService,
        AppDbContext context,
        ILogger<DeleteInvoiceRepository> logger)
    {
        _vkOrdClientFactory = vkOrdClientFactory;
        _cacheService = cacheService;
        _context = context;
        _logger = logger;
    }

    public async Task Delete(string externalId, CancellationToken cancellationToken)
    {
        var vkOrdCredential = await _vkOrdClientFactory.GetVkOrdCredentialAsync();
        var vkOrdClient = await _vkOrdClientFactory.CreateClient();

        // Удаляем из VK Ord API
        await vkOrdClient.DeleteInvoiceV3(externalId, cancellationToken);

        _logger.LogInformation($"Invoice {externalId} deleted from VK ORD API");

        // Удаляем из БД
        var invoice = await _context.VkOrdInvoices
            .FirstOrDefaultAsync(
                AppDbContext.DefaultGetVkOrd<VkOrdInvoice>(externalId, vkOrdCredential),
                cancellationToken);

        if (invoice != null)
        {
            _context.VkOrdInvoices.Remove(invoice);
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation($"Invoice {externalId} deleted from database");
        }

        // Инвалидируем кэш
        var cacheKey = GetCacheKey(externalId, vkOrdCredential);
        await _cacheService.Remove<VkOrdInvoice>(cacheKey, cancellationToken);
    }

    private string GetCacheKey(string externalId, ApiCredential apiCredential)
    {
        return $"vkord:{apiCredential.LogicalAccountId}:{externalId}:{EntityType.Invoice.GetDescription()}";
    }
}
