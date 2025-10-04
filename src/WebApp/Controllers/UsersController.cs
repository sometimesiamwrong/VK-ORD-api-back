using Domain.Data;
using Domain.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : BaseController
    {
        private readonly AppDbContext _db;

        public UsersController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet("me")]
        public async Task<UserProfileResponse> Me()
        {
            var userId = HttpContext.User.GetUserId();
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return null;
            }
            var result = new UserProfileResponse
            {
                Id = user.Id,
                UserName = user.UserName,
                Name = user.Name,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
            return result;
        }

        [HttpPatch("me")]
        public async Task<UserProfileResponse> UpdateMe([FromBody] UpdateUserRequest request)
        {
            var userId = HttpContext.User.GetUserId();
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return null;
            }
            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                user.Name = request.Name;
            }
            user.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            var result = new UserProfileResponse
            {
                Id = user.Id,
                UserName = user.UserName,
                Name = user.Name,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
            return result;
        }
    }

    public class UserProfileResponse
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string? Name { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class UpdateUserRequest
    {
        public string? Name { get; set; }
    }
}


