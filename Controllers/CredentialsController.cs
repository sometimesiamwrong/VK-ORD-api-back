using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VkOrdApiWrapper.Data;
using VkOrdApiWrapper.Entities;
using VkOrdApiWrapper.Extensions;
using VkOrdApiWrapper.Models.Responses;
using VkOrdApiWrapper.Security;

namespace VkOrdApiWrapper.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class ApiCredentialsController : BaseApiController
    {
        private readonly ApplicationDbContext _db;
        private readonly ISecretProtector _protector;

        public ApiCredentialsController(ApplicationDbContext db, ISecretProtector protector)
        {
            _db = db;
            _protector = protector;
        }

        [HttpGet]
        public async Task<ApiResponse<List<ApiCredentialResponse>>> Get()
        {
            var userId = HttpContext.User.GetUserId();
            var items = await _db.ApiCredentials.Where(c => c.UserId == userId).OrderBy(c => c.CreatedAt).ToListAsync();
            var data = items.Select(x => new ApiCredentialResponse
            {
                Id = x.Id,
                Environment = x.Environment,
                DisplayName = x.DisplayName,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            }).ToList();
            return Ok(data);
        }

        [HttpPost]
        public async Task<ApiResponse<ApiCredentialResponse>> Create([FromBody] CreateApiCredentialRequest request)
        {
            var userId = HttpContext.User.GetUserId();
            var enc = _protector.Encrypt(request.TokenPlain);
            var entity = new ApiCredential
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Environment = request.Environment,
                TokenEncrypted = enc,
                DisplayName = request.DisplayName,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _db.ApiCredentials.Add(entity);
            await _db.SaveChangesAsync();

            var resp = new ApiCredentialResponse
            {
                Id = entity.Id,
                Environment = entity.Environment,
                DisplayName = entity.DisplayName,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };
            return Ok(resp, "Credential saved");
        }

        [HttpPut("{id}")]
        public async Task<ApiResponse<ApiCredentialResponse>> Update(Guid id, [FromBody] UpdateApiCredentialRequest request)
        {
            var userId = HttpContext.User.GetUserId();
            var entity = await _db.ApiCredentials.FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);
            if (entity == null)
            {
                HttpContext.Response.StatusCode = 404;
                return Error<ApiCredentialResponse>("Not found");
            }
            if (!string.IsNullOrWhiteSpace(request.TokenPlain))
            {
                entity.TokenEncrypted = _protector.Encrypt(request.TokenPlain);
            }
            if (!string.IsNullOrWhiteSpace(request.Environment))
            {
                entity.Environment = request.Environment;
            }
            entity.DisplayName = request.DisplayName ?? entity.DisplayName;
            entity.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            var resp = new ApiCredentialResponse
            {
                Id = entity.Id,
                Environment = entity.Environment,
                DisplayName = entity.DisplayName,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };
            return Ok(resp, "Credential updated");
        }

        [HttpDelete("{id}")]
        public async Task<ApiResponse> Delete(Guid id)
        {
            var userId = HttpContext.User.GetUserId();
            var entity = await _db.ApiCredentials.FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);
            if (entity == null)
            {
                HttpContext.Response.StatusCode = 404;
                return Error("Not found");
            }
            _db.ApiCredentials.Remove(entity);
            await _db.SaveChangesAsync();
            return Ok("Credential deleted");
        }
    }

    public class ApiCredentialResponse
    {
        public Guid Id { get; set; }
        public string Environment { get; set; }
        public string? DisplayName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateApiCredentialRequest
    {
        public string Environment { get; set; }
        public string TokenPlain { get; set; }
        public string? DisplayName { get; set; }
    }

    public class UpdateApiCredentialRequest
    {
        public string? Environment { get; set; }
        public string? TokenPlain { get; set; }
        public string? DisplayName { get; set; }
    }
}


