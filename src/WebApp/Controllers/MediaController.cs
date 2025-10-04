using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<UploadMediaResponse> UploadMedia([FromForm] IFormFile file)
        {

            var request = new UploadMediaRequest
            {
                FileStream = file.OpenReadStream(),
                FileName = file.FileName,
                ContentType = file.ContentType
            };

            return await _vkOrdService.UploadMediaAsync(request, UserId);
        }

        /// <summary>
        /// Получить информацию о медиа файле
        /// </summary>
        [HttpGet("{externalId}")]
        public async Task<GetMediaResponse> GetMedia(string externalId)
        {
            return await _vkOrdService.GetMediaAsync(externalId, UserId);
        }

        /// <summary>
        /// Удалить медиа файл
        /// </summary>
        [HttpDelete("{externalId}")]
        public async Task<bool> DeleteMedia(string externalId)
        {
            return await _vkOrdService.DeleteMediaAsync(externalId, UserId);
        }
    }
}
