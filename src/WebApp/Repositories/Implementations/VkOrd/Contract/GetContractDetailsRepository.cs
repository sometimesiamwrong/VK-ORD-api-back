using Domain.Data;
using Domain.Entities.VkOrd;
using Microsoft.EntityFrameworkCore;
using WebApp.Models.Responses;
using WebApp.Repositories.Interfaces.VkOrd.Contract;
using WebApp.Services.Interfaces;

namespace WebApp.Repositories.Implementations.VkOrd.Contract;

/// <summary>
/// Репозиторий для получения полных деталей контракта
/// </summary>
public class GetContractDetailsRepository : IGetContractDetailsRepository
{
    private readonly AppDbContext _context;
    private readonly IVkOrdApiClientFactory _vkOrdApiClientFactory;
    private readonly ILogger<GetContractDetailsRepository> _logger;

    public GetContractDetailsRepository(AppDbContext context, IVkOrdApiClientFactory vkOrdApiClientFactory, ILogger<GetContractDetailsRepository> logger)
    {
        _context = context;
        _vkOrdApiClientFactory = vkOrdApiClientFactory;
        _logger = logger;
    }

    public async Task<GetContractDetailsDto?> GetDetailsAsync(
        string externalId,
        CancellationToken cancellationToken)
    {
        var vkOrdCredential = await _vkOrdApiClientFactory.GetVkOrdCredentialAsync();

        _logger.LogInformation("Getting contract details for ExternalId: {ExternalId}", externalId);

        // Получаем контракт со всеми связанными данными
        var contract = await _context.VkOrdContracts
            .Include(c => c.ContractParties)
                .ThenInclude(cp => cp.Counterparty)
            .Include(c => c.CreativeContracts)
                .ThenInclude(cc => cc.Creative)
                    .ThenInclude(c => c.CreativeMedia)
                        .ThenInclude(cm => cm.Media)
            .Include(c => c.AdditionalContracts)
            .FirstOrDefaultAsync(
                c => c.LogicalAccountId == vkOrdCredential.LogicalAccountId && c.ExternalId == externalId,
                cancellationToken);

        if (contract == null)
        {
            _logger.LogWarning("Contract not found. ExternalId: {ExternalId}", externalId);
            return null;
        }

        _logger.LogInformation(
            "Found contract with {PartiesCount} parties, {CreativesCount} creatives, {AdditionalCount} additional agreements",
            contract.ContractParties.Count,
            contract.CreativeContracts.Count,
            contract.AdditionalContracts.Count);

        // Маппируем в DTO с полными сущностями
        var dto = new GetContractDetailsDto
        {
            Contract = contract.Data,
            SyncStatus = contract.SyncStatus.ToString(),
            CreatedAt = contract.CreatedAt,
            UpdatedAt = contract.UpdatedAt,
            Parties = contract.ContractParties
                .Select(cp => cp.Counterparty)
                .Where(c => c != null)
                .Distinct()
                .ToList(),
            Creatives = contract.CreativeContracts
                .Select(cc => cc.Creative)
                .Where(c => c != null)
                .Distinct()
                .ToList(),
            AdditionalContracts = contract.AdditionalContracts
                .Select(ac => new AdditionalContractDto
                {
                    ExternalId = ac.ExternalId,
                    Serial = ac.Data?.Serial,
                    Date = ac.Data?.Date != null 
                        ? DateTime.TryParse(ac.Data.Date, out var date) ? date : null 
                        : null,
                    SyncStatus = ac.SyncStatus.ToString()
                })
                .ToList()
        };

        return dto;
    }
}
