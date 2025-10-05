using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VkOrdApi.Media;
using WebApp.Models.Requests;
using WebApp.Models.Responses;
using WebApp.Services.Interfaces;

namespace WebApp.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class MediaController : BaseController
    {
        private readonly IVkOrdService _vkOrdService;
        private readonly ILogger<MediaController> _logger;

        public MediaController(IVkOrdService vkOrdService, ILogger<MediaController> logger)
        {
            _vkOrdService = vkOrdService;
            _logger = logger;
        }

        /// <summary>
        /// Загрузить медиа файл в VK ОРД
        /// </summary>
        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task UploadMedia([FromForm] IFormFile file, CancellationToken cancellationToken)
        {

            var request = new UploadMediaRequest
            {
                FileStream = file.OpenReadStream(),
                FileName = file.FileName,
                ContentType = file.ContentType
            };

            await _vkOrdService.UploadMedia(request, cancellationToken);
        }

        /// <summary>
        /// Получить информацию о медиа файле
        /// </summary>
        [HttpGet("{externalId}")]
        public async Task<VkOrdMediaInfoResponse> GetMedia(string externalId, CancellationToken cancellationToken)
        {
            return await _vkOrdService.GetMedia(externalId, cancellationToken);    
        }

        /// <summary>
        /// Получить список медиа файлов
        /// </summary>
        [HttpGet("page")]
        public async Task<VkOrdMediaInfoListResponseDto> GetPageMedia(PageRequest pageRequest, CancellationToken cancellationToken)
        {
            return await _vkOrdService.GetPageMedia(pageRequest, cancellationToken);
        }
    }
}
