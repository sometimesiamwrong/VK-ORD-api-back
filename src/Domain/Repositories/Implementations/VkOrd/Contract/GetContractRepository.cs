using Domain.BrokenRules;
using Domain.Data;
using Domain.Entities;
using Domain.Entities.Enums;
using Domain.Entities.VkOrd;
using Domain.Extensions;
using Domain.Repositories.Interfaces.VkOrd.Contract;
using Domain.Repositories.Interfaces.VkOrd.Counterparty;
using Domain.Services.Interfaces;
using Domain.VkOrdApi.Contract;
using Microsoft.EntityFrameworkCore;

namespace Domain.Repositories.Implementations.VkOrd.Contract
{
    /// <summary>
    /// Репозиторий для получения контрактов VK ORD API
    /// </summary>
    public class GetContractRepository : IGetContractRepository
    {
        private readonly IVkOrdApiClientFactory _vkOrdClientFactory;
        private readonly Func<AppDbContext> _contextFactory;
        private readonly IGetCounterpartyByIdRepository _getCounterpartyByIdRepository;

        public GetContractRepository(
            IVkOrdApiClientFactory vkOrdClientFactory,
            Func<AppDbContext> contextFactory,
            IGetCounterpartyByIdRepository getCounterpartyByIdRepository)
        {
            _vkOrdClientFactory = vkOrdClientFactory;
            _contextFactory = contextFactory;
            _getCounterpartyByIdRepository = getCounterpartyByIdRepository;
        }

        public async Task<VkOrdContract> Get(string externalId, CancellationToken cancellationToken, bool noCache = false)
        {
            await using var context = _contextFactory();
            var vkOrdCredential = await _vkOrdClientFactory.GetVkOrdCredentialAsync();

                // Получаем данные из базы данных
            var data = await context.VkOrdContracts
                    .Include(x=>x.ContractParties)
                        .ThenInclude(x=>x.Counterparty)
                    .FirstOrDefaultAsync(AppDbContext.DefaultGetVkOrd<VkOrdContract>(externalId, vkOrdCredential), cancellationToken);

            if (data == null || noCache)
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
                await SaveToDatabase(data, cancellationToken, context);
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

        private async Task SaveToDatabase(VkOrdContract data, CancellationToken cancellationToken, AppDbContext context)
        {
            var customer = await _getCounterpartyByIdRepository.Get(data.Data.ClientExternalId, cancellationToken);
            var contractor = await _getCounterpartyByIdRepository.Get(data.Data.ContractorExternalId, cancellationToken);

            // Удаляем все старые связи для этого договора
            var existingParties = context.VkOrdContractParties.Where(cp => cp.ContractId == data.Id).ToList();
            
            // Удаляем все существующие связи
            foreach (var party in existingParties)
            {
                data.ContractParties.Remove(party);
                context.Remove(party);
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
                context.VkOrdContracts.Add(data);
            }
            else
            {
                // Обновляем данные в базу данных
                context.VkOrdContracts.Update(data);
            }

            await context.SaveChangesAsync(cancellationToken);
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
