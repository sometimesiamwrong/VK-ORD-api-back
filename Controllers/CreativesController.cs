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

            var apiContext = GetVkApiContext();
            var result = await _vkOrdService.CreateCreativeAsync(request, apiContext);

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
            var apiContext = GetVkApiContext();
            var result = await _vkOrdService.GetCreativeAsync(externalId, apiContext);

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
            var apiContext = GetVkApiContext();
            var result = await _vkOrdService.GetCreativeStatusAsync(externalId, apiContext);
            var statusResponse = CreativeStatusResponse.Create(externalId, result);
            return Ok(statusResponse, "Status retrieved successfully");
        }

        /// <summary>
        /// Удалить креатив
        /// </summary>
        [HttpDelete("{externalId}")]
        public async Task<ApiResponse> DeleteCreative(string externalId)
        {
            var apiContext = GetVkApiContext();
            var result = await _vkOrdService.DeleteCreativeAsync(externalId, apiContext);

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

            var apiContext = GetVkApiContext();
            var results = await _vkOrdService.CreateBulkCreativesAsync(requests, apiContext);
            var bulkResponse = BulkCreativeResponse.Create(results, requests.Count);
            return Ok(bulkResponse, "Bulk operation completed");
        }

        /// <summary>
        /// Проверить, что креатив прошел верификацию в ЕРИР
        /// </summary>
        [HttpGet("{externalId}/verify")]
        public async Task<ApiResponse<CreativeVerificationResponse>> VerifyCreative(string externalId, [FromQuery] int maxWaitMinutes = 120)
        {
            var apiContext = GetVkApiContext();
            var isVerified = await _vkOrdService.IsCreativeVerifiedAsync(externalId, apiContext, maxWaitMinutes);
            var verificationResponse = CreativeVerificationResponse.Create(externalId, isVerified);

            return Ok(verificationResponse,
                isVerified ? "Creative is verified" : "Creative verification check completed");
        }
    }
}