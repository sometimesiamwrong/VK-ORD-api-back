using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VkOrdApiWrapper.Data;
using VkOrdApiWrapper.Entities;
using VkOrdApiWrapper.Extensions;
using VkOrdApiWrapper.Models.Responses;

namespace VkOrdApiWrapper.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly ApplicationDbContext _db;

        public UsersController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet("me")]
        public async Task<ApiResponse<UserProfileResponse>> Me()
        {
            var userId = HttpContext.User.GetUserId();
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                HttpContext.Response.StatusCode = 404;
                return Error<UserProfileResponse>("User not found");
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
            return Ok(result);
        }

        [HttpPatch("me")]
        public async Task<ApiResponse<UserProfileResponse>> UpdateMe([FromBody] UpdateUserRequest request)
        {
            var userId = HttpContext.User.GetUserId();
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                HttpContext.Response.StatusCode = 404;
                return Error<UserProfileResponse>("User not found");
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
            return Ok(result, "Profile updated");
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


