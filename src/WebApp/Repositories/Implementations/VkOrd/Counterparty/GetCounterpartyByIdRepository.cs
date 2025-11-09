using Domain.BrokenRules;
using Domain.Data;
using Domain.Entities;
using Domain.Entities.Enums;
using Domain.Entities.VkOrd;
using Domain.Extensions;
using Domain.VkOrdApi.Person;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;
using WebApp.Models.Responses;
using WebApp.Repositories.Interfaces.VkOrd.Counterparty;
using WebApp.Services.Interfaces;

namespace WebApp.Repositories.Implementations.VkOrd.Counterparty
{
    /// <summary>
    /// Репозиторий для получения контрагента VK ORD API по ID
    /// </summary>
    public class GetCounterpartyByIdRepository : IGetCounterpartyByIdRepository
    {
        private readonly IVkOrdApiClientFactory _vkOrdClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppDbContext _context;

        public GetCounterpartyByIdRepository(
            IVkOrdApiClientFactory vkOrdClientFactory,
            AppDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _vkOrdClientFactory = vkOrdClientFactory;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<VkOrdCounterparty> Get(string externalId, CancellationToken cancellationToken)
        {
            var vkOrdCredential = await _vkOrdClientFactory.GetVkOrdCredentialAsync();
            var noCache = _httpContextAccessor.GetNoCacheHeader();
                
            // Получаем данные из базы данных
            var data = await _context.VkOrdCounterparties
                    .FirstOrDefaultAsync(
                    AppDbContext.DefaultGetVkOrd<VkOrdCounterparty>(externalId, vkOrdCredential), cancellationToken);

            if (data == null || data.IsExpired() || noCache)
            {
                // Получаем данные из API
                var vkOrdData = await GetByApi(externalId, cancellationToken);

                if (vkOrdData == null)
                {
                    throw BrokenRuleCodes.DataIsEmpty.AsExn();
                }

                // Мапим данные
                data = MapOperation(vkOrdData, data, vkOrdCredential, externalId);

                if(data.IsNew())
                {
                    // Добавляем данные в базу данных
                    _context.VkOrdCounterparties.Add(data);
                }
                else
                {
                    // Обновляем данные в базе данных
                    _context.VkOrdCounterparties.Update(data);
                }

                // Сохраняем данные в базу данных
                await _context.SaveChangesAsync(cancellationToken);
            }

            return data;
        }

        private VkOrdCounterparty MapOperation(VkOrdApiPersonResponse response, VkOrdCounterparty? data, ApiCredential vkOrdCredential, string externalId)
        {
            data ??= new VkOrdCounterparty()
            {
                LogicalAccountId = vkOrdCredential.LogicalAccountId,
                ExternalId = externalId
            };

            data.UpdateData(response);
            return data;
        }

        private async Task<VkOrdApiPersonResponse?> GetByApi(string externalId, CancellationToken cancellationToken)
        {
            var vkOrdClient = await _vkOrdClientFactory.CreateClient();

            var person = await vkOrdClient.GetPerson(externalId, cancellationToken);

            if (person == null)
            {
                return null;
            }

            return person;
        }

        private string GetCacheKey(string externalId, ApiCredential apiCredential)
        {
            return $"vkord:{apiCredential.LogicalAccountId}:{externalId}:{EntityType.Counterparty.GetDescription()}";
        }

    }
}
