using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Domain.Extensions;
using WebApp.Models.Requests;
using WebApp.Models.Responses;
using WebApp.Services.Interfaces;

namespace WebApp.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : BaseController
    {
        private readonly IUserService _service;

        public UsersController(IUserService service)
        {
            _service = service;
        }

        [HttpGet("me")]
        public async Task<UserProfileResponse?> Me(CancellationToken cancellationToken)
        {
            var userId = HttpContext.User.GetUserId();
            return await _service.Get(userId, cancellationToken);
        }

        [HttpPatch("me")]
        public async Task<UserProfileResponse?> UpdateMe([FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
        {
            var userId = HttpContext.User.GetUserId();
            return await _service.Update(userId, request, cancellationToken);
        }
    }
}


