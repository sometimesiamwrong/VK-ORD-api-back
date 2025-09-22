using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VkOrdApiWrapper.Models.Requests;
using VkOrdApiWrapper.Models.Responses;
using VkOrdApiWrapper.Services.Interfaces;
using VkOrdApiWrapper.Data;
using Microsoft.EntityFrameworkCore;
using VkOrdApiWrapper.Models.Entities;

namespace VkOrdApiWrapper.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class ContractsController : BaseApiController
    {
        private readonly IVkOrdService _vkOrdService;
        private readonly AppDbContext _db;
        private readonly ILogger<ContractsController> _logger;

        public ContractsController(IVkOrdService vkOrdService, AppDbContext db, ILogger<ContractsController> logger)
        {
            _vkOrdService = vkOrdService;
            _db = db;
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

        /// <summary>
        /// Получить контракт из локального хранилища по Id
        /// </summary>
        [HttpGet("by-id/{id:int}")]
        public async Task<ApiResponse<ContractEntity>> GetById(int id)
        {
            var entity = await _db.Contracts.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null)
            {
                HttpContext.Response.StatusCode = 404;
                return Error<ContractEntity>("Not found");
            }
            return Ok(entity, "Found");
        }

        /// <summary>
        /// Получить список контрактов (пагинация)
        /// </summary>
        [HttpGet]
        public async Task<ApiResponse<List<ContractEntity>>> GetList([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize <= 0 || pageSize > 200 ? 20 : pageSize;

            var items = await _db.Contracts
                .AsNoTracking()
                .OrderByDescending(x => x.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return Ok(items, "Ok");
        }

        /// <summary>
        /// Удалить контракт из локального хранилища по Id
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<ApiResponse> Delete(int id)
        {
            var entity = await _db.Contracts.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null)
            {
                HttpContext.Response.StatusCode = 404;
                return Error("Not found");
            }
            _db.Contracts.Remove(entity);
            await _db.SaveChangesAsync();
            return Ok("Deleted");
        }
    }
}
