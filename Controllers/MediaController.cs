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
    public class MediaController : BaseApiController
    {
        private readonly IVkOrdService _vkOrdService;
        private readonly ILogger<MediaController> _logger;

        public MediaController(IVkOrdService vkOrdService, ILogger<MediaController> logger)
        {
            _vkOrdService = vkOrdService;
            _logger = logger;
        }

        /// <summary>
        /// Извлекает контекст VK API из заголовков запроса
        /// </summary>
        private (Guid userId, string? env) GetContext() => (HttpContext.User.GetUserId(), Request.Headers["x-api-vk-env"].FirstOrDefault());

        /// <summary>
        /// Загрузить медиа файл в VK ОРД
        /// </summary>
        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<ApiResponse<UploadMediaResponse>> UploadMedia([FromForm] IFormFile file)
        {
            if (!ModelState.IsValid)
            {
                return ApiResponse<UploadMediaResponse>.Error("Invalid request data");
            }

            if (file == null || file.Length == 0)
            {
                return ApiResponse<UploadMediaResponse>.Error("File is required");
            }

            try
            {
                var (userId, env) = GetContext();

                var request = new UploadMediaRequest
                {
                    FileStream = file.OpenReadStream(),
                    FileName = file.FileName,
                    ContentType = file.ContentType
                };

                var result = await _vkOrdService.UploadMediaAsync(request, userId, env);

                return ApiResponse<UploadMediaResponse>.Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading media file");
                return ApiResponse<UploadMediaResponse>.Error($"Upload failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Получить информацию о медиа файле
        /// </summary>
        [HttpGet("{externalId}")]
        public async Task<ApiResponse<GetMediaResponse>> GetMedia(string externalId)
        {
            if (string.IsNullOrWhiteSpace(externalId))
            {
                return ApiResponse<GetMediaResponse>.Error("External ID is required");
            }

            try
            {
                var (userId, env) = GetContext();
                var result = await _vkOrdService.GetMediaAsync(externalId, userId, env);

                return ApiResponse<GetMediaResponse>.Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting media {ExternalId}", externalId);
                return ApiResponse<GetMediaResponse>.Error($"Get media failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Удалить медиа файл
        /// </summary>
        [HttpDelete("{externalId}")]
        public async Task<ApiResponse<bool>> DeleteMedia(string externalId)
        {
            if (string.IsNullOrWhiteSpace(externalId))
            {
                return ApiResponse<bool>.Error("External ID is required");
            }

            try
            {
                var (userId, env) = GetContext();
                var result = await _vkOrdService.DeleteMediaAsync(externalId, userId, env);

                if (result)
                {
                    return ApiResponse<bool>.Ok(true);
                }
                else
                {
                    return ApiResponse<bool>.Error("Delete media failed");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting media {ExternalId}", externalId);
                return ApiResponse<bool>.Error($"Delete media failed: {ex.Message}");
            }
        }
    }
}
