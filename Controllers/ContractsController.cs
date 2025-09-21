using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VkOrdApiWrapper.Models.Requests;
using VkOrdApiWrapper.Models.Responses;
using VkOrdApiWrapper.Services.Interfaces;

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

            var result = await _vkOrdService.CreateOrUpdateContractAsync(request);

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
            var result = await _vkOrdService.GetContractAsync(externalId);

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
