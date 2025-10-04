using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models.Requests;
using WebApp.Models.Responses;
using WebApp.Services.Interfaces;

namespace WebApp.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class CreativesController : BaseController
    {
        private readonly IVkOrdService _vkOrdService;
        private readonly ILogger<CreativesController> _logger;

        public CreativesController(IVkOrdService vkOrdService, ILogger<CreativesController> logger)
        {
            _vkOrdService = vkOrdService;
            _logger = logger;
        }
        
        /// <summary>
        /// Создать новый креатив в VK ОРД
        /// </summary>
        [HttpPost]
        public async Task<CreateCreativeResponse> CreateCreative([FromBody] CreateCreativeRequest request)
        {

            return await _vkOrdService.CreateCreativeAsync(request, UserId);
        }

        /// <summary>
        /// Получить информацию о креативе по external_id
        /// </summary>
        [HttpGet("{externalId}")]
        public async Task<CreateCreativeResponse> GetCreative(string externalId)
        {
            return await _vkOrdService.GetCreativeAsync(externalId, UserId);
        }

        /// <summary>
        /// Получить список креативов (итерация external_ids и сбор полных данных)
        /// </summary>
        [HttpGet]
        public async Task<GetCreativesResponse> GetCreatives([FromQuery] int? offset = null, [FromQuery] int? limit = null)
        {
            return await _vkOrdService.GetAllCreativesAsync(UserId, offset, limit);
        }

        /// <summary>
        /// Получить креатив по ERID
        /// </summary>
        [HttpGet("by-erid/{erid}")]
        public async Task<CreativeResponse> GetCreativeByErid(string erid)
        {
            return await _vkOrdService.GetCreativeByEridAsync(erid, UserId);
        }

        /// <summary>
        /// Получить статус обработки креатива в ЕРИР
        /// </summary>
        [HttpGet("{externalId}/status")]
        public async Task<CreativeStatusResponse> GetCreativeStatus(string externalId)
        {
            var result = await _vkOrdService.GetCreativeStatusAsync(externalId, UserId);
            return CreativeStatusResponse.Create(externalId, result);
        }

        /// <summary>
        /// Удалить креатив
        /// </summary>
        [HttpDelete("{externalId}")]
        public async Task<bool> DeleteCreative(string externalId)
        {
            return await _vkOrdService.DeleteCreativeAsync(externalId, UserId);
        }


        /// <summary>
        /// Создать несколько креативов одновременно
        /// </summary>
        [HttpPost("bulk")]
        public async Task<BulkCreativeResponse> CreateBulkCreatives([FromBody] List<CreateCreativeRequest> requests)
        {
            var results = await _vkOrdService.CreateBulkCreativesAsync(requests, UserId);
            return BulkCreativeResponse.Create(results, requests.Count);
        }

        /// <summary>
        /// Проверить, что креатив прошел верификацию в ЕРИР
        /// </summary>
        [HttpGet("{externalId}/verify")]
        public async Task<CreativeVerificationResponse> VerifyCreative(string externalId, [FromQuery] int maxWaitMinutes = 120)
        {
            var isVerified = await _vkOrdService.IsCreativeVerifiedAsync(externalId, UserId, maxWaitMinutes);
            return CreativeVerificationResponse.Create(externalId, isVerified);
        }
    }
}