using Domain.Entities.VkOrd;
using Domain.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models.Requests;
using WebApp.Models.Responses;
using WebApp.Services.Interfaces;

namespace WebApp.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class ContractsController : BaseController
    {
        private readonly IVkOrdService _vkOrdService;

        public ContractsController(IVkOrdService vkOrdService)
        {
            _vkOrdService = vkOrdService;
        }

        /// <summary>
        /// Создать или обновить контракт в VK ОРД
        /// </summary>
        [HttpPut("{externalId}")]
        public Task CreateOrUpdateContract(string externalId, [FromBody] CreateContractRequest request, CancellationToken cancellationToken)
        {
            // Устанавливаем externalId из маршрута
            request.ExternalId = externalId;

            return _vkOrdService.CreateOrUpdateContract(request, cancellationToken);
        }

        /// <summary>
        /// Получить информацию о контракте по external_id
        /// </summary>
        [HttpGet("{externalId}")]
        public Task<VkOrdContract> GetContract(string externalId, CancellationToken cancellationToken)
        {
            return _vkOrdService.GetContract(externalId, cancellationToken);
        }

    }
}
