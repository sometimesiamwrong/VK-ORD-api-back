using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VkOrdApiWrapper.Models.Requests;
using VkOrdApiWrapper.Models.Responses;
using VkOrdApiWrapper.Services.Interfaces;
using VkOrdApiWrapper.Models.VkOrd;
using VkOrdApiWrapper.Extensions;

namespace VkOrdApiWrapper.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class CreativesController : BaseApiController
    {
        private readonly IVkOrdService _vkOrdService;
        private readonly ILogger<CreativesController> _logger;

        public CreativesController(IVkOrdService vkOrdService, ILogger<CreativesController> logger)
        {
            _vkOrdService = vkOrdService;
            _logger = logger;
        }

        /// <summary>
        /// Извлекает контекст VK API из заголовков запроса
        /// </summary>
        private (Guid userId, string? env) GetContext() => (HttpContext.User.GetUserId(), Request.Headers["x-api-vk-env"].FirstOrDefault());

        /// <summary>
        /// Создать новый креатив в VK ОРД
        /// </summary>
        [HttpPost]
        public async Task<ApiResponse<CreateCreativeResponse>> CreateCreative([FromBody] CreateCreativeRequest request)
        {
            if (!ModelState.IsValid)
            {
                HttpContext.Response.StatusCode = 400;
                return Error<CreateCreativeResponse>("Invalid request data");
            }

            var (userId, env) = GetContext();
            var result = await _vkOrdService.CreateCreativeAsync(request, userId, env);

            if (result.Success)
            {
                return Ok(result, "Creative created successfully");
            }
            else
            {
                HttpContext.Response.StatusCode = 400;
                return Error<CreateCreativeResponse>(result.ErrorMessage);
            }
        }

        /// <summary>
        /// Получить информацию о креативе по external_id
        /// </summary>
        [HttpGet("{externalId}")]
        public async Task<ApiResponse<CreateCreativeResponse>> GetCreative(string externalId)
        {
            var (userId, env) = GetContext();
            var result = await _vkOrdService.GetCreativeAsync(externalId, userId, env);

            if (result.Success)
            {
                return Ok(result, "Creative found");
            }
            else
            {
                HttpContext.Response.StatusCode = 404;
                return Error<CreateCreativeResponse>(result.ErrorMessage ?? "Creative not found");
            }
        }

        /// <summary>
        /// Получить статус обработки креатива в ЕРИР
        /// </summary>
        [HttpGet("{externalId}/status")]
        public async Task<ApiResponse<CreativeStatusResponse>> GetCreativeStatus(string externalId)
        {
            var (userId, env) = GetContext();
            var result = await _vkOrdService.GetCreativeStatusAsync(externalId, userId, env);
            var statusResponse = CreativeStatusResponse.Create(externalId, result);
            return Ok(statusResponse, "Status retrieved successfully");
        }

        /// <summary>
        /// Удалить креатив
        /// </summary>
        [HttpDelete("{externalId}")]
        public async Task<ApiResponse> DeleteCreative(string externalId)
        {
            var (userId, env) = GetContext();
            var result = await _vkOrdService.DeleteCreativeAsync(externalId, userId, env);

            if (result)
            {
                HttpContext.Response.StatusCode = 200;
                return Ok("Creative deleted successfully");
            }
            else
            {
                HttpContext.Response.StatusCode = 404;
                return Error("Creative not found or could not be deleted");
            }
        }


        /// <summary>
        /// Создать несколько креативов одновременно
        /// </summary>
        [HttpPost("bulk")]
        public async Task<ApiResponse<BulkCreativeResponse>> CreateBulkCreatives([FromBody] List<CreateCreativeRequest> requests)
        {
            if (!ModelState.IsValid)
            {
                HttpContext.Response.StatusCode = 400;
                return Error<BulkCreativeResponse>("Invalid request data");
            }

            if (requests.Count > 50)
            {
                HttpContext.Response.StatusCode = 400;
                return Error<BulkCreativeResponse>("Maximum 50 creatives per request");
            }

            var (userId, env) = GetContext();
            var results = await _vkOrdService.CreateBulkCreativesAsync(requests, userId, env);
            var bulkResponse = BulkCreativeResponse.Create(results, requests.Count);
            return Ok(bulkResponse, "Bulk operation completed");
        }

        /// <summary>
        /// Проверить, что креатив прошел верификацию в ЕРИР
        /// </summary>
        [HttpGet("{externalId}/verify")]
        public async Task<ApiResponse<CreativeVerificationResponse>> VerifyCreative(string externalId, [FromQuery] int maxWaitMinutes = 120)
        {
            var (userId, env) = GetContext();
            var isVerified = await _vkOrdService.IsCreativeVerifiedAsync(externalId, userId, env, maxWaitMinutes);
            var verificationResponse = CreativeVerificationResponse.Create(externalId, isVerified);

            return Ok(verificationResponse,
                isVerified ? "Creative is verified" : "Creative verification check completed");
        }
    }
}