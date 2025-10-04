using Domain.Data;
using Domain.Entities;
using Domain.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Security;

namespace WebApp.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class CredentialsController : BaseController
    {
        private readonly AppDbContext _db;
        private readonly ISecretProtector _protector;

        public CredentialsController(AppDbContext db, ISecretProtector protector)
        {
            _db = db;
            _protector = protector;
        }

        [HttpGet]
        public async Task<List<ApiCredentialResponse>> Get()
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
            return data;
        }

        [HttpGet("{id}")]
        public async Task<ApiCredentialResponse> GetById(Guid id)
        {
            var userId = HttpContext.User.GetUserId();
            var item = await _db.ApiCredentials.FirstOrDefaultAsync(c => c.UserId == userId && c.Id == id);
            if (item is not null)
            {
                return new ApiCredentialResponse
                {
                    Id = item.Id,
                    Environment = item.Environment,
                    DisplayName = item.DisplayName,
                    CreatedAt = item.CreatedAt,
                    UpdatedAt = item.UpdatedAt
                };
            }

            return null;
        }

        [HttpPost]
        public async Task<ApiCredentialResponse> Create([FromBody] CreateApiCredentialRequest request)
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
            return resp;
        }

        [HttpPut("{id}")]
        public async Task<ApiCredentialResponse> Update(Guid id, [FromBody] UpdateApiCredentialRequest request)
        {
            var userId = HttpContext.User.GetUserId();
            var entity = await _db.ApiCredentials.FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);
            if (entity == null)
            {
                HttpContext.Response.StatusCode = 404;
                return null;
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
            return resp;
        }

        [HttpDelete("{id}")]
        public async Task Delete(Guid id)
        {
            var userId = HttpContext.User.GetUserId();
            var entity = await _db.ApiCredentials.FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);
            if (entity == null)
            {
                HttpContext.Response.StatusCode = 404;
                return;
            }
            _db.ApiCredentials.Remove(entity);
            await _db.SaveChangesAsync();
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


