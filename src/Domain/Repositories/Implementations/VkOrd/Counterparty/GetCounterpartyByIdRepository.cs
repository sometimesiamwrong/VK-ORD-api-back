using Domain.BrokenRules;
using Domain.Data;
using Domain.Entities;
using Domain.Entities.VkOrd;
using Domain.Extensions;
using Domain.Repositories.Interfaces.VkOrd.Counterparty;
using Domain.Services.Interfaces;
using Domain.VkOrdApi.Person;
using Microsoft.EntityFrameworkCore;

namespace Domain.Repositories.Implementations.VkOrd.Counterparty
{
    /// <summary>
    /// Репозиторий для получения контрагента VK ORD API по ID
    /// </summary>
    public class GetCounterpartyByIdRepository : IGetCounterpartyByIdRepository
    {
        private readonly IVkOrdApiClientFactory _vkOrdClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Func<AppDbContext> _contextFactory;

        public GetCounterpartyByIdRepository(
            IVkOrdApiClientFactory vkOrdClientFactory,
            Func<AppDbContext> contextFactory,
            IHttpContextAccessor httpContextAccessor)
        {
            _vkOrdClientFactory = vkOrdClientFactory;
            _contextFactory = contextFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<VkOrdCounterparty> Get(string externalId, CancellationToken cancellationToken, bool noCache = false)
        {
            await using var context = _contextFactory();
            var vkOrdCredential = await _vkOrdClientFactory.GetVkOrdCredentialAsync();
            var noCacheHeader = _httpContextAccessor.GetNoCacheHeader();
            var shouldBypassCache = noCache || noCacheHeader;

            // Получаем данные из базы данных
            var data = await context.VkOrdCounterparties
                    .FirstOrDefaultAsync(
                    AppDbContext.DefaultGetVkOrd<VkOrdCounterparty>(externalId, vkOrdCredential), cancellationToken);

            if (data == null || data.IsExpired() || shouldBypassCache)
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
                    await context.VkOrdCounterparties.AddAsync(data, cancellationToken);
                }
                else
                {
                    context.VkOrdCounterparties.Update(data);
                }

                await context.SaveChangesAsync(cancellationToken);
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
    }
}
