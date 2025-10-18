using Domain;
using Domain.VkOrdApi.Media;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
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
        [HttpPost("v1/upload")]
        [Consumes("multipart/form-data")]
        [SwaggerOperation(Summary = "Загрузить медиа файл", Description = "Загружает медиа файл в VK ОРД")]
        [SwaggerResponse(200, "Файл успешно загружен")]
        public async Task<string> UploadMedia([FromForm] UploadFileDto uploadFile, CancellationToken cancellationToken)
        {
            var request = new UploadMediaRequest
            {
                FileStream = uploadFile.File.OpenReadStream(),
                FileName = uploadFile.File.FileName,
                ContentType = uploadFile.File.ContentType
            };

            return await _vkOrdService.UploadMedia(request, cancellationToken);
        }

        /// <summary>
        /// Получить информацию о медиа файле
        /// </summary>
        [HttpGet("v1/{externalId}")]
        public async Task<VkOrdApiMediaInfoResponse> GetMedia(string externalId, CancellationToken cancellationToken)
        {
            return await _vkOrdService.GetMedia(externalId, cancellationToken);    
        }

        /// <summary>
        /// Получить список медиа файлов
        /// </summary>
        [HttpGet("v1/page")]
        public async Task<VkOrdMediaInfoListResponseDto> GetPageMedia(PageRequest pageRequest, CancellationToken cancellationToken)
        {
            return await _vkOrdService.GetPageMedia(pageRequest, cancellationToken);
        }
    }
}
