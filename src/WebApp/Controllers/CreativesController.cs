using Domain;
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
        [HttpPost]
        public Task<VkOrdApiCreativeV3RequestResponse> CreateCreative([FromBody] CreateCreativeRequest request, CancellationToken cancellationToken)
        {
            return _vkOrdService.CreateCreative(request, cancellationToken);
        }

        /// <summary>
        /// Получить информацию о креативе по external_id
        /// </summary>
        [HttpGet("{externalId}")]
        public Task<VkOrdApiCreativeV3Response> GetCreative(string externalId, CancellationToken cancellationToken)
        {
            return _vkOrdService.GetCreative(externalId, cancellationToken);
        }

        /// <summary>
        /// Получить список креативов (итерация external_ids и сбор полных данных)
        /// </summary>
        [HttpGet]
        public Task<GetCreativesResponse> GetCreatives([FromQuery] PageRequest pageRequest, CancellationToken cancellationToken)
        {
            return _vkOrdService.GetPageCreatives(pageRequest, cancellationToken);
        }

        /// <summary>
        /// Получить креатив по ERID
        /// </summary>
        [HttpGet("by-erid/{erid}")]
        public Task<VkOrdApiCreativeV3Response> GetCreativeByErid(string erid, CancellationToken cancellationToken)
        {
            return _vkOrdService.GetCreativeByErid(erid, cancellationToken);
        }
    }
}