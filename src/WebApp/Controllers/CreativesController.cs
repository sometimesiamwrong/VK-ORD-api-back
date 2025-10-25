using Domain;
using Domain.Entities.VkOrd;
using Domain.VkOrdApi.Creative;
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
        [HttpPost("v1")]
        public Task<VkOrdCreative> CreateCreative([FromBody] CreateCreativeRequest request, CancellationToken cancellationToken)
        {
            return _vkOrdService.CreateCreative(request, cancellationToken);
        }

        /// <summary>
        /// Получить информацию о креативе по external_id
        /// </summary>
        [HttpGet("v1/{externalId}")]
        public Task<VkOrdCreative> GetCreative(string externalId, CancellationToken cancellationToken)
        {
            return _vkOrdService.GetCreative(externalId, cancellationToken);
        }

        /// <summary>
        /// Получить список креативов (итерация external_ids и сбор полных данных)
        /// </summary>
        [HttpGet("v1")]
        public Task<GetCreativesResponse> GetCreatives([FromQuery] PageRequest pageRequest, CancellationToken cancellationToken)
        {
            return _vkOrdService.GetPageCreatives(pageRequest, cancellationToken);
        }

        /// <summary>
        /// Получить креатив по ERID
        /// </summary>
        [HttpGet("v1/by-erid/{erid}")]
        public Task<VkOrdCreative> GetCreativeByErid(string erid, CancellationToken cancellationToken)
        {
            return _vkOrdService.GetCreativeByErid(erid, cancellationToken);
        }
    }
}