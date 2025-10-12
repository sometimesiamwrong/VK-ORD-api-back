using Domain.BrokenRules;
using Domain.Data;
using Domain.Entities;
using Domain.Entities.Enums;
using Domain.Entities.VkOrd;
using Domain.Extensions;
using Domain.VkOrdApi.Contract;
using Microsoft.EntityFrameworkCore;
using WebApp.Models.Responses;
using WebApp.Repositories.Interfaces.VkOrd.Contract;
using WebApp.Repositories.Interfaces.VkOrd.Counterparty;
using WebApp.Services.Interfaces;

namespace WebApp.Repositories.Implementations.VkOrd.Contract
{
    /// <summary>
    /// Репозиторий для получения контрактов VK ORD API
    /// </summary>
    public class GetContractRepository : IGetContractRepository
    {
        private readonly IVkOrdApiClientFactory _vkOrdClientFactory;
        private readonly ICacheService _cacheService;
        private readonly AppDbContext _context;
        private readonly IGetCounterpartyByIdRepository _getCounterpartyByIdRepository;
        private readonly ILogger<GetContractRepository> _logger;

        public GetContractRepository(
            IVkOrdApiClientFactory vkOrdClientFactory,
            ICacheService cacheService,
            AppDbContext context,
            ILogger<GetContractRepository> logger,
            IGetCounterpartyByIdRepository getCounterpartyByIdRepository)
        {
            _vkOrdClientFactory = vkOrdClientFactory;
            _cacheService = cacheService;
            _context = context;
            _logger = logger;
            _getCounterpartyByIdRepository = getCounterpartyByIdRepository;
        }

        public async Task<VkOrdContract> Get(string externalId, CancellationToken cancellationToken)
        {
            var vkOrdCredential = await _vkOrdClientFactory.GetVkOrdCredentialAsync();

            var cacheKey = GetCacheKey(externalId, vkOrdCredential);

            // Получаем данные из кэша
            var data = await _cacheService.Get<VkOrdContract>(
                cacheKey,
                cancellationToken
            );

            // Если данные не найдены в кэше, то получаем данные из базы данных
            if (data == null)
            {
                // Получаем данные из базы данных
                data = await _context.VkOrdContracts
                    .Include(x=>x.ContractParties)
                        .ThenInclude(x=>x.Counterparty)
                    .FirstOrDefaultAsync(AppDbContext.DefaultGetVkOrd<VkOrdContract>(externalId, vkOrdCredential), cancellationToken);

                if (data == null)
                {
                    // Получаем данные из API
                    var vkOrdData = await GetByApi(externalId, cancellationToken);

                    if (vkOrdData == null)
                    {
                        throw BrokenRuleCodes.DataIsEmpty.AsExn();
                    }

                    // Мапим данные
                    data = MapOperation(vkOrdData, data, vkOrdCredential, externalId);

                    // Сохраняем данные в базу данных
                    await SaveToDatabase(data, cancellationToken);
                }
                            
                // Сохраняем данные в кэш
                await _cacheService.Save(cacheKey, data, cancellationToken);
            }

            return data;
        }

        private VkOrdContract MapOperation(VkOrdApiContractResponse response, VkOrdContract? data, ApiCredential vkOrdCredential, string externalId)
        {
            data ??= new VkOrdContract()
            {
                LogicalAccountId = vkOrdCredential.LogicalAccountId,
                ExternalId = externalId
            };

            data.UpdateData(response);
            return data;
        }

        private async Task SaveToDatabase(VkOrdContract data, CancellationToken cancellationToken)
        {
            var customer = await _getCounterpartyByIdRepository.Get(data.Data.ClientExternalId, cancellationToken);
            var contractor = await _getCounterpartyByIdRepository.Get(data.Data.ContractorExternalId, cancellationToken);

            // Удаляем все старые связи для этого договора
            var existingParties = _context.VkOrdContractParties.Where(cp => cp.ContractId == data.Id).ToList();
            
            // Удаляем все существующие связи
            foreach (var party in existingParties)
            {
                data.ContractParties.Remove(party);
                _context.Remove(party);
            }
            
            // Добавляем новые связи если они есть
            if (customer != null)
            {
                data.ContractParties.Add(new VkOrdContractParty
                {
                    ContractId = data.Id,
                    CounterpartyId = customer.Id,
                    Role = VkOrdContractRole.Customer,
                    CreatedAt = DateTimeOffset.UtcNow
                });
            }
            
            if (contractor != null)
            {
                data.ContractParties.Add(new VkOrdContractParty
                {
                    ContractId = data.Id,
                    CounterpartyId = contractor.Id,
                    Role = VkOrdContractRole.Contractor,
                    CreatedAt = DateTimeOffset.UtcNow
                });
            }

            if(data.IsNew())
            {
                // Добавляем данные в базу данных
                _context.VkOrdContracts.Add(data);
            }
            else
            {
                // Обновляем данные в базу данных
                _context.VkOrdContracts.Update(data);
            }

            await _context.SaveChangesAsync(cancellationToken);
        }

        private async Task<VkOrdApiContractResponse?> GetByApi(string externalId, CancellationToken cancellationToken)
        {
            var vkOrdClient = await _vkOrdClientFactory.CreateClient();

            var contract = await vkOrdClient.GetContractByExternalId(externalId, cancellationToken);

            if (contract == null)
            {
                return null;
            }

            return contract;
        }

        private string GetCacheKey(string externalId, ApiCredential apiCredential)
        {
            return $"vkord:{apiCredential.LogicalAccountId}:{externalId}:{EntityType.Contract.GetDescription()}";
        }
    }
}
