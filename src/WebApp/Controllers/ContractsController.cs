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
        /// Извлекает контекст VK API из заголовков запроса
        /// </summary>
        private (Guid userId, string? env) GetContext() => (HttpContext.User.GetUserId(), Request.Headers["x-api-vk-env"].FirstOrDefault());

        /// <summary>
        /// Создать или обновить контракт в VK ОРД
        /// </summary>
        [HttpPut("{externalId}")]
        public async Task<CreateContractResponse> CreateOrUpdateContract(string externalId, [FromBody] CreateContractRequest request)
        {
            // Устанавливаем externalId из маршрута
            request.ExternalId = externalId;

            var (userId, env) = GetContext();
            return await _vkOrdService.CreateOrUpdateContractAsync(request, userId, env);
        }

        /// <summary>
        /// Получить информацию о контракте по external_id
        /// </summary>
        [HttpGet("{externalId}")]
        public async Task<ContractResponse> GetContract(string externalId)
        {
            var (userId, env) = GetContext();
            return await _vkOrdService.GetContractAsync(externalId, userId, env);
        }

    }
}
