using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VkOrdApiWrapper.Models.Requests;
using VkOrdApiWrapper.Models.Responses;
using VkOrdApiWrapper.Services.Interfaces;
using VkOrdApiWrapper.Data;
using VkOrdApiWrapper.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace VkOrdApiWrapper.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class CreativesController : BaseApiController
    {
        private readonly IVkOrdService _vkOrdService;
        private readonly AppDbContext _db;
        private readonly ILogger<CreativesController> _logger;

        public CreativesController(IVkOrdService vkOrdService, AppDbContext db, ILogger<CreativesController> logger)
        {
            _vkOrdService = vkOrdService;
            _db = db;
            _logger = logger;
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

            var result = await _vkOrdService.CreateCreativeAsync(request);

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
            var result = await _vkOrdService.GetCreativeAsync(externalId);

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
            var result = await _vkOrdService.GetCreativeStatusAsync(externalId);
            var statusResponse = CreativeStatusResponse.Create(externalId, result);
            return Ok(statusResponse, "Status retrieved successfully");
        }

        /// <summary>
        /// Удалить креатив
        /// </summary>
        [HttpDelete("{externalId}")]
        public async Task<ApiResponse> DeleteCreative(string externalId)
        {
            var result = await _vkOrdService.DeleteCreativeAsync(externalId);

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
        /// Получить креатив из локальной БД по Id
        /// </summary>
        [HttpGet("by-id/{id:int}")]
        public async Task<ApiResponse<CreativeEntity>> GetById(int id)
        {
            var entity = await _db.Creatives.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null)
            {
                HttpContext.Response.StatusCode = 404;
                return Error<CreativeEntity>("Not found");
            }
            return Ok(entity, "Found");
        }

        /// <summary>
        /// Получить список креативов (пагинация)
        /// </summary>
        [HttpGet]
        public async Task<ApiResponse<List<CreativeEntity>>> GetList([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize <= 0 || pageSize > 200 ? 20 : pageSize;

            var items = await _db.Creatives
                .AsNoTracking()
                .OrderByDescending(x => x.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return Ok(items, "Ok");
        }

        /// <summary>
        /// Удалить креатив из локальной БД по Id
        /// </summary>
        [HttpDelete("local/{id:int}")]
        public async Task<ApiResponse> DeleteLocal(int id)
        {
            var entity = await _db.Creatives.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null)
            {
                HttpContext.Response.StatusCode = 404;
                return Error("Not found");
            }
            _db.Creatives.Remove(entity);
            await _db.SaveChangesAsync();
            return Ok("Deleted");
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

            var results = await _vkOrdService.CreateBulkCreativesAsync(requests);
            var bulkResponse = BulkCreativeResponse.Create(results, requests.Count);
            return Ok(bulkResponse, "Bulk operation completed");
        }

        /// <summary>
        /// Проверить, что креатив прошел верификацию в ЕРИР
        /// </summary>
        [HttpGet("{externalId}/verify")]
        public async Task<ApiResponse<CreativeVerificationResponse>> VerifyCreative(string externalId, [FromQuery] int maxWaitMinutes = 120)
        {
            var isVerified = await _vkOrdService.IsCreativeVerifiedAsync(externalId, maxWaitMinutes);
            var verificationResponse = CreativeVerificationResponse.Create(externalId, isVerified);

            return Ok(verificationResponse,
                isVerified ? "Creative is verified" : "Creative verification check completed");
        }
    }
}