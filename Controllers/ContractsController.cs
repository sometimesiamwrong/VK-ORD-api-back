using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VkOrdApiWrapper.Models.Requests;
using VkOrdApiWrapper.Models.Responses;
using VkOrdApiWrapper.Services.Interfaces;
using VkOrdApiWrapper.Models.VkOrd;

namespace VkOrdApiWrapper.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class ContractsController : BaseApiController
    {
        private readonly IVkOrdService _vkOrdService;
        private readonly ILogger<ContractsController> _logger;

        public ContractsController(IVkOrdService vkOrdService, ILogger<ContractsController> logger)
        {
            _vkOrdService = vkOrdService;
            _logger = logger;
        }

        /// <summary>
        /// Извлекает контекст VK API из заголовков запроса
        /// </summary>
        private VkApiContext GetVkApiContext()
        {
            var apiKey = Request.Headers["x-api-vk-key"].FirstOrDefault();
            var route = Request.Headers["x-api-vk-route"].FirstOrDefault();

            if (string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(route))
            {
                throw new BadHttpRequestException("Missing required headers: x-api-vk-key and x-api-vk-route");
            }

            return new VkApiContext
            {
                ApiKey = apiKey,
                Route = route
            };
        }

        /// <summary>
        /// Создать или обновить контракт в VK ОРД
        /// </summary>
        [HttpPut("{externalId}")]
        public async Task<ApiResponse<CreateContractResponse>> CreateOrUpdateContract(string externalId, [FromBody] CreateContractRequest request)
        {
            // Устанавливаем externalId из маршрута
            request.ExternalId = externalId;

            if (!ModelState.IsValid)
            {
                HttpContext.Response.StatusCode = 400;
                return Error<CreateContractResponse>("Invalid request data");
            }

            var apiContext = GetVkApiContext();
            var result = await _vkOrdService.CreateOrUpdateContractAsync(request, apiContext);

            if (result.Success)
            {
                return Ok(result, "Contract created/updated successfully");
            }
            else
            {
                HttpContext.Response.StatusCode = 400;
                return Error<CreateContractResponse>(result.ErrorMessage);
            }
        }

        /// <summary>
        /// Получить информацию о контракте по external_id
        /// </summary>
        [HttpGet("{externalId}")]
        public async Task<ApiResponse<ContractResponse>> GetContract(string externalId)
        {
            var apiContext = GetVkApiContext();
            var result = await _vkOrdService.GetContractAsync(externalId, apiContext);

            if (result.Success)
            {
                return Ok(result, "Contract found");
            }
            else
            {
                HttpContext.Response.StatusCode = 404;
                return Error<ContractResponse>(result.Message ?? "Contract not found");
            }
        }

    }
}
